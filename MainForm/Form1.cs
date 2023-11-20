using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MainForm
{
    public partial class Form1 : Form
    {
        private BinarySearchTree binaryTree;
        private TextBox txtExpression;
        private Button btnCalculate;
        private Label lblResult;

        public Form1()
        {
            InitializeComponent();
            binaryTree = new BinarySearchTree();
            InitializeUI();
        }

        private void InitializeUI()
        {
            // TextBox để nhập biểu thức
            txtExpression = new TextBox();
            txtExpression.Location = new System.Drawing.Point(10, 10);
            txtExpression.Size = new System.Drawing.Size(200, 20);

            // Button để tính toán biểu thức
            btnCalculate = new Button();
            btnCalculate.Text = "Calculate";
            btnCalculate.Location = new System.Drawing.Point(220, 10);
            btnCalculate.Click += btnCalculate_Click;

            // Label để hiển thị kết quả
            lblResult = new Label();
            lblResult.Location = new System.Drawing.Point(10, 40);
            lblResult.Size = new System.Drawing.Size(200, 20);

            // Thêm các control vào form
            Controls.Add(txtExpression);
            Controls.Add(btnCalculate);
            Controls.Add(lblResult);
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                string expression = txtExpression.Text;
                double result = EvaluateExpression(expression);
                lblResult.Text = "Result: " + result.ToString();
            }
            catch (Exception ex)
            {
                lblResult.Text = "Error: " + ex.Message;
            }
        }

        private double EvaluateExpression(string expression)
        {
            binaryTree.BuildExpressionTree(expression);
            return binaryTree.Root.Evaluate();
        }

        private class BinarySearchTree
        {
            public class Node
            {
                public Node LeftNode { get; set; }
                public Node RightNode { get; set; }
                public char Operator { get; set; }
                public double? Operand { get; set; }

                public double Evaluate()
                {
                    if (Operand.HasValue)
                        return Operand.Value;

                    double leftValue = LeftNode.Evaluate();
                    double rightValue = RightNode.Evaluate();

                    switch (Operator)
                    {
                        case '+':
                            return leftValue + rightValue;
                        case '-':
                            return leftValue - rightValue;
                        case '*':
                            return leftValue * rightValue;
                        case '/':
                            if (rightValue == 0)
                                throw new InvalidOperationException("Division by zero");
                            return leftValue / rightValue;
                        default:
                            throw new InvalidOperationException("Invalid operator");
                    }
                }
            }

            public Node Root { get; set; }

            public void BuildExpressionTree(string expression)
            {
                Stack<Node> operandStack = new Stack<Node>();
                Stack<Node> operatorStack = new Stack<Node>();

                foreach (char c in expression)
                {
                    if (char.IsDigit(c))
                    {
                        Node operand = new Node { Operand = double.Parse(c.ToString()) };
                        operandStack.Push(operand);
                    }
                    else if (IsOperator(c))
                    {
                        Node currentOperator = new Node { Operator = c };

                        while (operatorStack.Count > 0 && GetOperatorPriority(c) <= GetOperatorPriority(operatorStack.Peek().Operator))
                        {
                            Node topOperator = operatorStack.Pop();
                            topOperator.RightNode = operandStack.Pop();
                            topOperator.LeftNode = operandStack.Pop();
                            operandStack.Push(topOperator);
                        }

                        operatorStack.Push(currentOperator);
                    }
                    else if (c == '(')
                    {
                        operatorStack.Push(new Node { Operator = c });
                    }
                    else if (c == ')')
                    {
                        while (operatorStack.Count > 0 && operatorStack.Peek().Operator != '(')
                        {
                            Node topOperator = operatorStack.Pop();
                            topOperator.RightNode = operandStack.Pop();
                            topOperator.LeftNode = operandStack.Pop();
                            operandStack.Push(topOperator);
                        }

                        // Pop '('
                        if (operatorStack.Count > 0)
                            operatorStack.Pop();
                    }
                }

                // Xử lý toán tử và toán hạng còn lại
                while (operatorStack.Count > 0)
                {
                    Node topOperator = operatorStack.Pop();
                    topOperator.RightNode = operandStack.Pop();
                    topOperator.LeftNode = operandStack.Pop();
                    operandStack.Push(topOperator);
                }

                // Đảm bảo rằng có một kết quả duy nhất trên đỉnh operandStack
                if (operandStack.Count == 1)
                    Root = operandStack.Pop();
                else
                    throw new InvalidOperationException("Invalid expression");
            }

            private bool IsOperator(char c)
            {
                return c == '+' || c == '-' || c == '*' || c == '/';
            }

            private int GetOperatorPriority(char op)
            {
                switch (op)
                {
                    case '+':
                    case '-':
                        return 1;
                    case '*':
                    case '/':
                        return 2;
                    default:
                        return 0;
                }
            }
        }
    }
}
