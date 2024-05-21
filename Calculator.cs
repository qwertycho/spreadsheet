namespace SpreadSheet.Calculator
{
    public class Calculator
    {
        public decimal Caluculate(string expression)
        {
            Stack<char> stack = new Stack<char>();
            string _exp = "";

            expression = expression.Replace(" ", "");
            for (int i = 0; i < expression.Length; i++)
            {
                char item = expression[i];

                if (char.IsDigit(item))
                {
                    _exp += item;
                    continue;
                }

                if (item == '(')
                {
                    if (i > 0 && char.IsDigit(expression[i - 1]))
                    {
                        stack.Push('*');
                    }
                    stack.Push(item);
                    continue;
                }

                if (item == ')')
                {
                    while (stack.Count > 0)
                    {
                        var sItem = stack.Pop();
                        if (sItem == '(')
                        {
                            break;
                        }
                        else
                        {
                            _exp += sItem;
                        }
                    }
                    continue;
                }

                while (stack.Count > 0)
                {

                    var sItem = stack.Pop();
                    if (sItem == '(' || sItem == ')')
                    {
                        stack.Push(sItem);
                        break;
                    }

                    var sOp = Operators.Get(sItem);
                    var iOp = Operators.Get(item);

                    if (sOp == null || iOp == null)
                    {
                        break;
                    }

                    if (sOp.Precedence >= iOp.Precedence)
                    {
                        _exp += sItem;
                    }
                    else
                    {
                        stack.Push(sItem);
                        break;
                    }

                }
                stack.Push(item);
            }
            while (stack.Count > 0)
            {
                _exp += stack.Pop();
            }

            List<string> tokens = new List<string>();
            tokens = _exp.Select(c => c.ToString()).ToList();

            try
            {
                while (true && tokens.Count > 2)
                {
                    for (int i = 0; i < tokens.Count; i++)
                    {
                        var op = Operators.Get(tokens[i]);
                        if (op != null)
                        {
                            var l = decimal.Parse(tokens[i - 2].ToString());
                            var r = decimal.Parse(tokens[i - 1].ToString());
                            var result = op.Invoke((l, r));
                            tokens.RemoveRange(i - 2, 3);
                            tokens.Insert(i - 2, result.ToString());
                            break;
                        }
                    }
                }
                decimal final = 0;
                for (int x = 0; x < tokens.Count; x++)
                {
                    final += decimal.Parse(tokens[x]);
                }
                Console.WriteLine($"Result: {final}");
                return final;
            }
            catch
            {
                throw new Exception("Invalid Expression");
            }
        }

    }

    public static class Operators
    {
        public static Operator? Get(string c)
        {
            if (c.Length > 1)
            {
                return null;
            }
            return Get(c[0]);
        }
        public static Operator? Get(char c)
        {
            return OperatorsArr.FirstOrDefault(x => x.Char == c);
        }
        public static Operator[] OperatorsArr = new Operator[]
        {
        new Operator('+', 1, (x) => x.left + x.right),
        new Operator('-', 1, (x) => x.left - x.right),
        new Operator('*', 2, (x) => x.left * x.right),
        new Operator('/', 2, (x) => x.left / x.right)
        };
    }

    public class Operator
    {
        public char Char { get; set; }
        public int Precedence { get; set; }
        public Func<(decimal left, decimal right), decimal> Invoke { get; set; }

        public Operator(char c, int p, Func<(decimal left, decimal right), decimal> i)
        {
            Char = c;
            Precedence = p;
            Invoke = i;
        }
    }
}
