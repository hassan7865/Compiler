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
    LINEEND,
    LPARENTHESES,
    RPARENTHESES,
    COMMA,
    SEMICOLON,
    LCURLYBRACES,
    RCURLYBRACES,
    UNKNOWN,
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
            {TokenTypes.LINEEND,@";" },
            {TokenTypes.LPARENTHESES, @"\(" },
            {TokenTypes.RPARENTHESES, @"\)" },
            {TokenTypes.LCURLYBRACES, @"\{" },
            {TokenTypes.RCURLYBRACES, @"\}" },
            {TokenTypes.SEMICOLON,@":" },
            {TokenTypes.COMMA,@"," },
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



   

class Tokenizer
{
    static void Main(string[] args)
    {
        var sourceCode = @"
            int num,num1;
            num = 7;
            int result = (4.67*7);

         ";



        Lexer lexer = new Lexer(sourceCode);
        List<Token> tokens = lexer.Tokenize();

        Console.WriteLine("Tokens:\n");
        int totalNumofTokens = 0;
        foreach (var token in tokens)
        {
            totalNumofTokens++;
            Console.WriteLine($"{token.Type}:{token.Value}");
        }
      
        Console.WriteLine($"\nTotal Number of tokens are:{totalNumofTokens}");

    }
}
