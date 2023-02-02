using System.ComponentModel.Design.Serialization;

double main_function(double x)
{
    double a = 5;
    if (x >= 0)
    {
        return Math.Sin(a / 2 * x) + Math.Pow(x * a, 1.0/3.0);
    }
    else
    {
        return Math.Sin(a / 2 * x) - Math.Pow(-x * a, 1.0/3.0);
    }
}

double[] x = new Double[]{-4, -2, 0, 2, 4};

int size = x.Length;
double[,] table = new double[size, size];
for (int i = 0; i < size; i++)
{
    table[i, 0] = main_function(x[i]);
}

for (int j = 1; j < size; j++)
{
    for (int i = 0; i < size - j; i++)
    {
        table[i, j] = (table[i + 1, j - 1] - table[i, j - 1]) / (x[i + j] - x[i]);
    }
}
Console.WriteLine("Newton method");

int decimals = 8;
Console.Write("y = {0}", Math.Round(table[0, 0], decimals));
for (int j = 1; j < size; j++)
{
    if (table[0, j] < 0)
    {
        Console.Write(" {0}", Math.Round(table[0, j], decimals));
        for (int x_i = 0; x_i < j; x_i++)
        {
            if (-x[x_i] < 0)
            {
                Console.Write(" * (x {0})", -x[x_i]);
            }
            else
            {
                Console.Write(" * (x + {0})", -x[x_i]);
            }
        }
    }
    else
    {
        Console.Write(" + {0}", Math.Round(table[0, j], decimals));
        for (int x_i = 0; x_i < j; x_i++)
        {
            if (-x[x_i] < 0)
            {
                Console.Write(" * (x {0})", -x[x_i]);
            }
            else
            {
                Console.Write(" * (x + {0})", -x[x_i]);
            }
        }
    }
}

Console.Write("\n\nSpline cub equations method");

double[,] equations = new double[4 * size, 4 * size + 1];
// a1 b1 c1 d1 ...

for (int i = 0; i < size - 1; i++) // Value in fn(xn)
{
    equations[i, 4 * i] = 1; // a
    equations[i, 4 * (size - 1)] = main_function(x[i]); // S
}

for (int i = 0; i < size - 2; i++) // fn(xn+1) = fn+1(xn+1)
{
    equations[i + size - 1, 4 * i] = 1; // an
    equations[i + size - 1, 4 * i + 1] = x[i + 1] - x[i]; // bn
    equations[i + size - 1, 4 * i + 2] = Math.Pow(x[i + 1] - x[i], 2); // cn
    equations[i + size - 1, 4 * i + 3] = Math.Pow(x[i + 1] - x[i], 3); // dn
    
    equations[i + size - 1, 4 * (i + 1)] = -1; // an+1
}

for (int i = 0; i < size - 2; i++) // fn'(xn+1) = fn+1'(xn+1)
{
    equations[i + 2 * (size - 1) - 1, 4 * i + 1] = 1; // bn
    equations[i + 2 * (size - 1) - 1, 4 * i + 2] = 2 * (x[i+1] - x[i]); // cn
    equations[i + 2 * (size - 1) - 1, 4 * i + 3] = 3 * Math.Pow(x[i+1] - x[i], 2); // dn
    
    equations[i + 2 * (size - 1) - 1, 4 * i + 5] = -1; // bn+1
}
for (int i = 0; i < size - 2; i++) // fn''(xn+1) = fn+1''(xn+1)
{
    equations[i + 3 * (size - 1) - 2, 4 * i + 2] = 1; // cn
    equations[i + 3 * (size - 1) - 2, 4 * i + 3] = 3 * (x[i+1] - x[i]); // dn
    
    equations[i + 3 * (size - 1) - 2, 4 * i + 6] = -1; // cn+1
}
equations[4 * (size - 1) - 3, 4 * (size - 1)] = main_function(x[size - 1]); // S
equations[4 * (size - 1) - 3, 4 * (size - 1) - 1] = Math.Pow(x[size - 1] - x[size - 2], 3); // d
equations[4 * (size - 1) - 3, 4 * (size - 1) - 2] = Math.Pow(x[size - 1] - x[size - 2], 2); // c
equations[4 * (size - 1) - 3, 4 * (size - 1) - 3] = x[size - 1] - x[size - 2]; // b
equations[4 * (size - 1) - 3, 4 * (size - 1) - 4] = 1; // a

equations[4 * (size - 1) - 2, 2] = 1; // c

equations[4 * (size - 1) - 1, 4 * (size - 1) - 2] = 1; //cn
equations[4 * (size - 1) - 1, 4 * (size - 1) - 1] = 3 * (x[size - 1] - x[size - 2]);

double[] param = gaussian(equations);

for (int i = 0; i < size - 1; i++)
{
    Console.WriteLine("\nSection {0}; x [{1}, {2}]", i + 1, x[i], x[i + 1]);
    Console.Write("y = {0}", Math.Round(param[i * 4], decimals));
    if (Math.Round(param[i * 4 + 1], decimals) == 0)
    {
        
    }
    else if (param[i * 4 + 1] < 0)
    {
        if (x[i] < 0)
        {
            Console.Write(" {0}(x + {1})", Math.Round(param[i * 4 + 1], decimals), -Math.Round(x[i], decimals));
        }
        else
        {
            Console.Write(" {0}(x - {1})", Math.Round(param[i * 4 + 1], decimals), Math.Round(x[i], decimals));
        }
    }
    else
    {
        if (x[i] < 0)
        {
            Console.Write(" + {0}(x + {1})", Math.Round(param[i * 4 + 1], decimals), -Math.Round(x[i], decimals));
        }
        else
        {
            Console.Write(" + {0}(x - {1})", Math.Round(param[i * 4 + 1], decimals), Math.Round(x[i], decimals));
        }
    }
    
    
    if (Math.Round(param[i * 4 + 2], decimals) == 0)
    {
        
    }
    else if (param[i * 4 + 2] < 0)
    {
        if (x[i] < 0)
        {
            Console.Write(" {0}(x + {1})^2", Math.Round(param[i * 4 + 2], decimals), -Math.Round(x[i], decimals));
        }
        else
        {
            Console.Write(" {0}(x - {1})^2", Math.Round(param[i * 4 + 2], decimals), Math.Round(x[i], decimals));
        }
    }
    else
    {
        if (x[i] < 0)
        {
            Console.Write(" + {0}(x + {1})^2", Math.Round(param[i * 4 + 2], decimals), -Math.Round(x[i], decimals));
        }
        else
        {
            Console.Write(" + {0}(x - {1})^2", Math.Round(param[i * 4 + 2], decimals), Math.Round(x[i], decimals));
        }
    }
    
    
    if (Math.Round(param[i * 4 + 3], decimals) == 0)
    {
        
    }
    else if (param[i * 4 + 3] < 0)
    {
        if (x[i] < 0)
        {
            Console.Write(" {0}(x + {1})^3", Math.Round(param[i * 4 + 3], decimals), -Math.Round(x[i], decimals));
        }
        else
        {
            Console.Write(" {0}(x - {1})^3", Math.Round(param[i * 4 + 3], decimals), Math.Round(x[i], decimals));
        }
    }
    else
    {
        if (x[i] < 0)
        {
            Console.Write(" + {0}(x + {1})^3", Math.Round(param[i * 4 + 3], decimals), -Math.Round(x[i], decimals));
        }
        else
        {
            Console.Write(" + {0}(x - {1})^3", Math.Round(param[i * 4 + 3], decimals), Math.Round(x[i], decimals));
        }
    }
}


void view_equations(double[,] equations)
{
    for (int i = 0; i < 3 * size; i++)
    {
        for (int j = 0; j < 3 * size + 1; j++)
        {
            Console.Write("{0} ", equations[i, j]);
        }
        Console.WriteLine();
    }
}

double[] gaussian(double[,] equations)
{
    for (int a = 0; a < (size - 1) * 4 - 1; a++)
    {
        if (equations[a, a] == 0)
        {
            for (int i = a; i < (size - 1) * 4; i++)
            {
                if (equations[i, a] != 0)
                {
                    row_switch_equations(equations, a, i);
                    break;
                }
            }
        }
        double k;
        for (int i = a + 1; i < (size - 1) * 4; i++)
        {
            k = equations[i, a] / equations[a, a];
            sub_row_equations(equations, i, a, k);
        }
    }

    double[] res = new double[(size - 1) * 4];

    for (int i = (size - 1) * 4 - 1; i >= 0; i--)
    {
        double value = equations[i, 4 * (size - 1)];
        for (int j = i; j < (size - 1) * 4 - 1; j++)
        {
            value -= equations[i, j + 1] * res[j + 1];
        }
        res[i] = value / equations[i, i];
    }

    return res;
}

void sub_row_equations(double[,] equations, int row1, int row2, double k)
{
    for (int j = 0; j < size * 4; j++)
    {
        equations[row1, j] -= k * equations[row2, j];
    }
}

void row_switch_equations(double[,] equations, int row1, int row2)
{
    for (int j = 0; j < size * 4; j++)
    {
        double element = equations[row1, j];
        equations[row1, j] = equations[row2, j];
        equations[row2, j] = element;
    }
}