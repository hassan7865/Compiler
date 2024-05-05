using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum TokenTypes
{
    INT,
    BOOL,
    FLOAT,
    IDENTIFIER,
    INTEGER,
    OPERATORS,
    ASSIGNMENT,
    SEMICOLON,
    UNKNOWN,
    CONSOLE,
    FLOATING_POINT,
    BOOL_LITERAL
}

public class Token
{
    public TokenTypes Type { get; set; }
    public string Value { get; set; }

    public Token(TokenTypes type, string value)
    {
        Type = type;
        Value = value;
    }
}

public class Lexer
{
    private string _sourceCode;
    private List<Token> _tokens;

    public Lexer(string sourceCode)
    {
        _sourceCode = sourceCode;
        _tokens = new List<Token>();
    }

    public List<Token> Tokenize()
    {
        var KEYWORDS = new Dictionary<TokenTypes, string>
        {
            {TokenTypes.INT, @"\bint\b" },
            {TokenTypes.FLOAT, @"\bfloat\b" },
            {TokenTypes.BOOL, @"\bbool\b" },
            {TokenTypes.BOOL_LITERAL, @"\b(?:true|false)\b" },
            {TokenTypes.CONSOLE, @"\bconsole\b" },
            {TokenTypes.IDENTIFIER, @"[a-zA-Z_]\w*" },
            {TokenTypes.ASSIGNMENT, @"=" },
            {TokenTypes.FLOATING_POINT, @"\b\d+\.\d+\b" },
            {TokenTypes.INTEGER, @"\b\d+\b" },
            {TokenTypes.OPERATORS, @"[\+\-\*\/]" },
        };

        var CombinedREGEX = string.Join("|", KEYWORDS.Values);

        var inpmatches = Regex.Matches(_sourceCode, CombinedREGEX);
        foreach (Match match in inpmatches)
        {
            var tokenType = GetTokenType(match.Value, KEYWORDS);
            _tokens.Add(new Token(tokenType, match.Value));
        }
        return _tokens;
    }

    public TokenTypes GetTokenType(string token, Dictionary<TokenTypes, string> pattern)
    {
        foreach (var Kvp in pattern)
        {
            if (Regex.IsMatch(token, Kvp.Value))
            {
                return Kvp.Key;
            }
        }
        return TokenTypes.UNKNOWN;
    }
}

public class Parser
{
    private readonly List<Token> _tokens;
    private int _currentIndex;
    private Token _currentToken;
    private Dictionary<string, TokenTypes> Variables;

    public Parser(List<Token> token)
    {
        _tokens = token;
        _currentIndex = 0;
        _currentToken = _tokens[_currentIndex];
        Variables = new Dictionary<string, TokenTypes>();
    }

    private void Advance()
    {
        _currentIndex++;
        if (_currentIndex < _tokens.Count)
        {
            _currentToken = _tokens[_currentIndex];
        }
    }

    private void ConsumeToken(TokenTypes tokenType)
    {
        if (_currentToken.Type == tokenType)
        {
            Advance();
        }
        else
        {
            ReportError("Invalid syntax. Expected: " + tokenType);
        }
    }

    private void ReportError(string message)
    {
        throw new Exception("Error: " + message);
    }

    public void Parse()
    {
        while (_currentIndex < _tokens.Count)
        {
            Declaration();
            Assign();
        }
    }

    private void Declaration()
    {
        if (_currentToken.Type == TokenTypes.INT || _currentToken.Type == TokenTypes.FLOAT || _currentToken.Type == TokenTypes.BOOL)
        {
            var variableType = _currentToken.Type;
            ConsumeToken(variableType);

            if (_currentToken.Type == TokenTypes.IDENTIFIER)
            {
                var variableName = _currentToken.Value;
                ConsumeToken(TokenTypes.IDENTIFIER);

                Console.WriteLine($"Variable declared: Type={variableType}, Name={variableName}");


                Variables.Add(variableName, variableType);
            }
            else
            {
                throw new Exception("Invalid variable name");
            }
        }
        else
        {
            throw new Exception("Invalid declaration");
        }
    }

    private void Assign()
    {
        if (_currentToken.Type == TokenTypes.IDENTIFIER)
        {
            var variableName = _currentToken.Value;
            ConsumeToken(_currentToken.Type);

            if (_currentToken.Type == TokenTypes.ASSIGNMENT)
            {
                ConsumeToken(TokenTypes.ASSIGNMENT);

                if (Variables.TryGetValue(variableName, out TokenTypes variableType))
                {
                    if ((_currentToken.Type == TokenTypes.INTEGER && variableType == TokenTypes.INT) ||
                        (_currentToken.Type == TokenTypes.FLOATING_POINT && variableType == TokenTypes.FLOAT) ||
                        (_currentToken.Type == TokenTypes.BOOL_LITERAL && variableType == TokenTypes.BOOL))
                    {
                        OutputVariableAssignment(variableName);
                    }
                    else
                    {
                        throw new Exception($"Invalid assignment value cannot assign {_currentToken.Type} into {variableType}");
                    }
                }
                else
                {
                    throw new Exception("Variable not found");
                }
            }
            else
            {
                throw new Exception("Invalid assignment");
            }
        }
        else
        {
            throw new Exception("Invalid assignment");
        }
    }

    private void OutputVariableAssignment(string variableName)
    {
        var value = _currentToken.Value;
        ConsumeToken(_currentToken.Type);

        Console.WriteLine($"Variable assigned: Name={variableName}, Value={value}");
    }

    private void Print()
    {
        if (_currentToken.Type == TokenTypes.CONSOLE)
        {
            ConsumeToken(TokenTypes.CONSOLE);
            if (_currentToken.Type == TokenTypes.IDENTIFIER)
            {
                var variableName = _currentToken.Value;
                ConsumeToken(TokenTypes.IDENTIFIER);

                Console.WriteLine($"Print variable: Name={variableName}");
            }
            else
            {
                throw new Exception("Invalid print statement");
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        //var sourceCode = @"
        //    int num;
        //    num = 7;
        // ";

        var sourceCode = @"
            int num1 = 1;int num2 = 2

        ";

        Lexer lexer = new Lexer(sourceCode);
        List<Token> tokens = lexer.Tokenize();

        Console.WriteLine("Tokens:");
        foreach (var token in tokens)
        {
            Console.WriteLine($"{token.Type}:{token.Value}");
        }

        Parser parser = new Parser(tokens);
        parser.Parse();
    }
}
