using System.Text.RegularExpressions;


public enum TokenTypes
{
    INTEGER,
    BOOL,
    FLOAT,
    IDENTIFIER,
    NUMBER,
    OPERATORS,
    ASSIGNMENT,
    SEMICOLON,
    UNKNOWN,
    CONSOLE

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
         {TokenTypes.INTEGER, @"\bint\b" },
         {TokenTypes.FLOAT, @"\bfloat\b" },
         {TokenTypes.BOOL, @"\b(?:true|false)\b" },
         {TokenTypes.CONSOLE,@"\bconsole\b" },
         {TokenTypes.IDENTIFIER, @"[a-zA-Z_]\w*" },
         { TokenTypes.ASSIGNMENT, @"=" },
         {TokenTypes.NUMBER, @"\d+(\.\d+)?" },
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

    public Parser(List<Token> token)
    {
        _tokens = token;
        _currentIndex = 0;
        _currentToken = _tokens[_currentIndex];
    }

    private void Advance()
    {
        _currentIndex++;
        if(_currentIndex < _tokens.Count)
        {
            _currentToken = _tokens[_currentIndex];
        }
    }


    private void ConsumeToken(TokenTypes tokenType)
    {
        if(_currentToken.Type == tokenType)
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
            Declaration_Assign();
        }

    }

    private void Declaration_Assign()
    {
        if (_currentToken.Type == TokenTypes.INTEGER || _currentToken.Type == TokenTypes.BOOL || _currentToken.Type == TokenTypes.FLOAT)

        {
            var variableType = _currentToken.Type;
            ConsumeToken(variableType);

            if (_currentToken.Type == TokenTypes.IDENTIFIER)
            {
                var variableName = _currentToken.Value;
                ConsumeToken(TokenTypes.IDENTIFIER);
                Console.WriteLine($"Variable declared: Type={variableType}, Name={variableName}");

                //ASSIGN

                if (_currentToken.Type == TokenTypes.ASSIGNMENT)
                {
                    ConsumeToken(TokenTypes.ASSIGNMENT);


                    if (_currentToken.Type == TokenTypes.NUMBER)
                    {
                        if (variableType == TokenTypes.INTEGER)
                        {
                            var value = _currentToken.Value;
                            ConsumeToken(_currentToken.Type);

                            Console.WriteLine($"Variable assigned: Name={variableName}, Value={value}");
                        }

                    }
                    else
                    {
                        throw new Exception($"Invalid assignment value cannot assign {_currentToken.Type} into {variableType}");
                    }
                }
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

}

class Program
{
    static void Main(string[] args)
    {
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