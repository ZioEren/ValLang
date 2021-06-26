using System.Collections.Generic;
using System;
using Microsoft.VisualBasic;

public class Lexer
{
    public string text, fn;
    public Position pos;
    public char current_char;

    public Lexer(string fn, string text)
    {
        this.fn = fn;
        this.text = text;
        this.pos = new Position(-1, 0, -1, fn, text);
        this.current_char = default(char);
        this.advance();
    }

    public void advance()
    {
        this.pos.advance(this.current_char);
        this.current_char = this.pos.idx < this.text.Length ? this.text[this.pos.idx] : default(char);
    }

    public Tuple<List<Token>, Error> make_tokens()
    {
        List<Token> tokens = new List<Token>();

        while (this.current_char != default(char))
        {
            if (this.current_char == ' ' || this.current_char == '\t')
            {
                this.advance();
            }
            else if (this.current_char == '#')
            {
                this.skip_comment();
            }
            else if (this.current_char == '↩' || this.current_char == ';')
            {
                tokens.Add(new Token(TokenType.NEWLINE, pos_start: this.pos));
                this.advance();
            }
            else if (this.current_char == '+')
            {
                Position pos_start = this.pos.copy();
                this.advance();

                if (this.current_char == '=')
                {
                    tokens.Add(new Token(TokenType.PLUS_EQ, pos_start: this.pos, pos_end: this.pos));
                    this.advance();
                }
                else if (this.current_char == '+')
                {
                    tokens.Add(new Token(TokenType.DOUBLE_PLUS, pos_start: this.pos, pos_end: this.pos));
                    this.advance();
                }
                else
                {
                    tokens.Add(new Token(TokenType.PLUS, pos_start: this.pos, pos_end: this.pos));
                }      
            }
            else if (this.current_char == '-')
            {
                Position pos_start = this.pos.copy();

                this.advance();

                if (this.current_char == '>')
                {
                    this.advance();
                    tokens.Add(new Token(TokenType.ARROW, pos_start: pos_start, pos_end: this.pos));
                }
                else if (this.current_char == '=')
                {
                    this.advance();
                    tokens.Add(new Token(TokenType.MINUS_EQ, pos_start: pos_start, pos_end: this.pos));
                }
                else if (this.current_char == '-')
                {
                    tokens.Add(new Token(TokenType.DOUBLE_MINUS, pos_start: pos_start, pos_end: this.pos));
                    this.advance();
                }
                else
                {
                    tokens.Add(new Token(TokenType.MINUS, pos_start: pos_start, pos_end: this.pos));
                }          
            }
            else if (this.current_char == '*')
            {
                Position pos_start = this.pos.copy();
                this.advance();

                if (this.current_char == '=')
                {
                    tokens.Add(new Token(TokenType.MUL_EQ, pos_start: this.pos, pos_end: this.pos));
                    this.advance();
                }
                else
                {
                    tokens.Add(new Token(TokenType.MUL, pos_start: this.pos, pos_end: this.pos));
                }     
            }
            else if (this.current_char == '/')
            {
                Position pos_start = this.pos.copy();
                this.advance();

                if (this.current_char == '=')
                {
                    tokens.Add(new Token(TokenType.DIV_EQ, pos_start: this.pos, pos_end: this.pos));
                    this.advance();
                }
                else if (this.current_char == '/')
                {
                    this.skip_comment();
                    continue;
                }
                else if (this.current_char == '*')
                {
                    this.advance();
                    bool firstChar = false;

                    while (true)
                    {
                        this.advance();

                        if (this.current_char == '*' && !firstChar)
                        {
                            firstChar = true;
                            continue;
                        }
                        else if (this.current_char == '*' && firstChar)
                        {
                            firstChar = false;
                        }
                        if (firstChar)
                        {
                            if (this.current_char == '/')
                            {
                                break;
                            }
                        }
                    }

                    this.advance();
                    continue;
                }
                else
                {
                    tokens.Add(new Token(TokenType.DIV, pos_start: this.pos, pos_end: this.pos));
                }
            }
            else if (this.current_char == '^')
            {
                Position pos_start = this.pos.copy();
                this.advance();

                if (this.current_char == '=')
                {
                    tokens.Add(new Token(TokenType.POW_EQ, pos_start: this.pos, pos_end: this.pos));
                    this.advance();
                }

                tokens.Add(new Token(TokenType.POW, pos_start: this.pos, pos_end: this.pos));
            }
            else if (this.current_char == '(')
            {
                tokens.Add(new Token(TokenType.LPAREN, pos_start: this.pos));
                this.advance();
            }
            else if (this.current_char == ':')
            {
                tokens.Add(new Token(TokenType.COLON, pos_start: this.pos));
                this.advance();
            }
            else if (this.current_char == ')')
            {
                tokens.Add(new Token(TokenType.RPAREN, pos_start: this.pos));
                this.advance();
            }
            else if (this.current_char == '[')
            {
                tokens.Add(new Token(TokenType.LSQUARE, pos_start: this.pos));
                this.advance();
            }
            else if (this.current_char == ']')
            {
                tokens.Add(new Token(TokenType.RSQUARE, pos_start: this.pos));
                this.advance();
            }
            else if ("0123456789".Contains(this.current_char.ToString()))
            {
                string num_str = "";
                int dot_count = 0;
                Position pos_start = this.pos.copy();

                while (this.current_char != default(char) && ("0123456789.").Contains(this.current_char.ToString()))
                {
                    if (this.current_char == '.')
                    {
                        if (dot_count == 1)
                        {
                            break;
                        }

                        dot_count += 1;
                        num_str += ".";
                    }
                    else
                    {
                        num_str += this.current_char;
                    }

                    this.advance();
                }

                if (dot_count == 0)
                {
                    tokens.Add(new Token(TokenType.INT, int.Parse(num_str), pos_start, this.pos));
                }
                else
                {
                    tokens.Add(new Token(TokenType.FLOAT, float.Parse(num_str.Replace(".", ",")), pos_start, this.pos));
                }
            }
            else if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(this.current_char.ToString()))
            {
                string id_str = "";
                Position pos_start = this.pos.copy();
                bool key = false;

                while (this.current_char != default(char) && ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_").Contains(this.current_char.ToString()))
                {
                    id_str += this.current_char;
                    this.advance();
                }

                foreach (string keyword in new string[] { "var", "and", "or", "not", "if", "then", "elif", "else", "for", "to", "step", "while", "fun", "end", "return", "continue", "break", "del", "do", "foreach", "in", "switch", "case", "default", "const", "struct", "goto", "namespace", "import", "use", "async" })
                {
                    if (id_str == keyword)
                    {
                        tokens.Add(new Token(TokenType.KEYWORD, id_str, pos_start, this.pos));
                        key = true;
                        break;
                    }
                }

                if (!key)
                {
                    tokens.Add(new Token(TokenType.IDENTIFIER, id_str, pos_start, this.pos));
                }
            }
            else if (this.current_char == '!')
            {
                Position pos_start = this.pos.copy();
                this.advance();

                if (this.current_char == '=')
                {
                    this.advance();

                    tokens.Add(new Token(TokenType.NE, pos_start: pos_start, pos_end: this.pos));
                    continue;
                }

                tokens.Add(new Token(TokenType.KEYWORD, "not", pos_start, this.pos));
            }
            else if (this.current_char == '=')
            {
                Position pos_start = this.pos.copy();

                this.advance();

                if (this.current_char == '=')
                {
                    this.advance();
                    tokens.Add(new Token(TokenType.EE, pos_start: pos_start, pos_end: this.pos));
                }
                else
                {
                    tokens.Add(new Token(TokenType.EQ, pos_start: pos_start, pos_end: this.pos));
                }        
            }
            else if (this.current_char == '<')
            {
                Position pos_start = this.pos.copy();

                this.advance();

                if (this.current_char == '=')
                {
                    this.advance();
                    tokens.Add(new Token(TokenType.LTE, pos_start: pos_start, pos_end: this.pos));
                }
                else if (this.current_char == '<')
                {
                    this.advance();

                    if (this.current_char == '=')
                    {
                        this.advance();
                        tokens.Add(new Token(TokenType.LEFT_SHIFT_EQ, pos_start: pos_start, pos_end: this.pos));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.LEFT_SHIFT, pos_start: pos_start, pos_end: this.pos));
                    }
                }
                else
                {
                    tokens.Add(new Token(TokenType.LT, pos_start: pos_start, pos_end: this.pos));
                }
            }
            else if (this.current_char == '>')
            {
                Position pos_start = this.pos.copy();

                this.advance();

                if (this.current_char == '=')
                {
                    this.advance();
                    tokens.Add(new Token(TokenType.GTE, pos_start: pos_start, pos_end: this.pos));
                }
                else if (this.current_char == '>')
                {
                    this.advance();

                    if (this.current_char == '=')
                    {
                        this.advance();
                        tokens.Add(new Token(TokenType.RIGHT_SHIFT_EQ, pos_start: pos_start, pos_end: this.pos));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.RIGHT_SHIFT, pos_start: pos_start, pos_end: this.pos));
                    }
                }
                else
                {
                    tokens.Add(new Token(TokenType.GT, pos_start: pos_start, pos_end: this.pos));
                }
            }
            else if (this.current_char == ',')
            {
                tokens.Add(new Token(TokenType.COMMA, pos_start: this.pos));
                this.advance();
            }
            else if (this.current_char == '"')
            {
                tokens.Add(this.make_string('"'));
            }
            else if (this.current_char.ToString() == "'")
            {
                tokens.Add(this.make_string(char.Parse("'")));
            }
            else if (this.current_char == '&')
            {
                Position pos_start = this.pos.copy();
                this.advance();

                if (this.current_char == '&')
                {
                    this.advance();
                    tokens.Add(new Token(TokenType.KEYWORD, "and", pos_start, this.pos));
                }
                else if (this.current_char == '=')
                {
                    this.advance();
                    tokens.Add(new Token(TokenType.LOGIC_AND_EQ, pos_start: pos_start, pos_end: this.pos));
                }
                else
                {
                    tokens.Add(new Token(TokenType.LOGIC_AND, pos_start: pos_start, pos_end: this.pos));
                }
            }
            else if (this.current_char == '|')
            {
                Position pos_start = this.pos.copy();
                this.advance();

                if (this.current_char == '|')
                {
                    this.advance();
                    tokens.Add(new Token(TokenType.KEYWORD, "or", pos_start, this.pos));
                }
                else if (this.current_char == '=')
                {
                    this.advance();
                    tokens.Add(new Token(TokenType.LOGIC_OR_EQ, pos_start: pos_start, pos_end: this.pos));
                }
                else
                {
                    tokens.Add(new Token(TokenType.LOGIC_OR, pos_start: pos_start, pos_end: this.pos));
                }
            }
            else if (this.current_char == '{')
            {
                tokens.Add(new Token(TokenType.LBRACE, pos_start: this.pos));
                tokens.Add(new Token(TokenType.NEWLINE, pos_start: this.pos));

                this.advance();
            }
            else if (this.current_char == '}')
            {
                tokens.Add(new Token(TokenType.RBRACE, pos_start: this.pos));
                tokens.Add(new Token(TokenType.NEWLINE, pos_start: this.pos));
                
                this.advance();
            }
            else if (this.current_char == '~')
            {
                Position pos_start = this.pos.copy();
                this.advance();

                if (this.current_char == '=')
                {
                    tokens.Add(new Token(TokenType.LOGIC_NOT_EQ, pos_start: this.pos, pos_end: this.pos));
                    this.advance();
                }
                else
                {
                    tokens.Add(new Token(TokenType.LOGIC_NOT, pos_start: this.pos, pos_end: this.pos));
                }            
            }
            else if (this.current_char == '%')
            {
                Position pos_start = this.pos.copy();
                this.advance();
                string tok_type = "MODULO";

                if (this.current_char == '=')
                {
                    tokens.Add(new Token(TokenType.MODULO_EQ, pos_start: this.pos, pos_end: this.pos));
                    this.advance();
                }
                else
                {
                    tokens.Add(new Token(TokenType.MODULO, pos_start: this.pos, pos_end: this.pos));
                }        
            }
            else if (this.current_char == '.')
            {
                tokens.Add(new Token(TokenType.DOT, pos_start: this.pos));
                this.advance();
            }
            else
            {
                Position pos_start = this.pos.copy();

                char theChar = this.current_char;
                this.advance();

                return new Tuple<List<Token>, Error>(null, new IllegalCharError(pos_start, this.pos, "'" + theChar + "'"));
            }
        }

        tokens.Add(new Token(TokenType.EOF, pos_start: this.pos));

        return new Tuple<List<Token>, Error>(tokens, null);
    }

    public Token make_string(char conclude_char)
    {
        string str = "";
        Position pos_start = this.pos.copy();
        bool escape_character = false;

        this.advance();

        Dictionary<char, char> escape_characters = new Dictionary<char, char>();

        escape_characters.Add('n', '\n');
        escape_characters.Add('t', '\t');
        escape_characters.Add('r', '\r');
        escape_characters.Add('\\', '\\');
        escape_characters.Add('a', '\a');
        escape_characters.Add('b', '\b');
        escape_characters.Add('f', '\f');
        escape_characters.Add('v', '\v');
        escape_characters.Add('"', '\"');
        escape_characters.Add('\'', '\'');

        while (this.current_char != default(char) && (this.current_char != conclude_char || escape_character))
        {
            if (escape_character)
            {
                try
                {
                    str += escape_characters[this.current_char];
                }
                catch 
                {
                    str += this.current_char;
                }
            }
            else
            {
                if (this.current_char == '\\')
                {
                    escape_character = true;
                    this.advance();
                    continue;
                }
                else
                {
                    str += this.current_char;
                }
            }

            this.advance();
            escape_character = false;
        }

        this.advance();

        return new Token(TokenType.STRING, str, pos_start, this.pos);
    }

    public void skip_comment()
    {
        this.advance();

        while (this.current_char != '\n' && this.current_char != default(char) && this.current_char != '↩')
        {
            this.advance();
        }

        this.advance();
    }
}