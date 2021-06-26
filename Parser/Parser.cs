using System.Collections.Generic;
using System;

public class Parser
{
    public List<Token> tokens;
    public int tok_idx = -1;
    public Token current_tok;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
        this.advance();
    }

    public ParseResult parse()
    {
        ParseResult res = this.statements();

        if (res.error == null && !this.current_tok.type.Equals(TokenType.EOF))
        {
            return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Token cannot appear after previous tokens"));
        }

        return res;
    }

    public void advance()
    {
        this.tok_idx++;
        this.update_current_tok();
    }

    public void reverse(int amount = 1)
    {
        this.tok_idx -= amount;
        this.update_current_tok();
    }

    public void update_current_tok()
    {
        if (this.tok_idx >= 0 && this.tok_idx < this.tokens.Count)
        {
            this.current_tok = this.tokens[this.tok_idx];
        }
    }

    public ParseResult atom()
    {
        ParseResult res = new ParseResult();
        Token tok = this.current_tok;

        if (tok.type.Equals(TokenType.INT) || tok.type.Equals(TokenType.FLOAT))
        {
            res.register_advancement();
            this.advance();

            return res.success(new NumberNode(tok));
        }
        else if (tok.type.Equals(TokenType.STRING))
        {
            res.register_advancement();
            this.advance();

            return res.success(new StringNode(tok));
        }
        else if (tok.type.Equals(TokenType.LBRACE))
        {
            List<object> elements = new List<object>();
            Position pos_start = this.current_tok.pos_start;

            res.register_advancement();
            this.advance();

            res.register_advancement();
            this.advance();

            if (this.current_tok.type.Equals(TokenType.RBRACE))
            {
                goto rbrace;
            }

            object value = res.register(this.expr());

            if (res.error != null)
            {
                return res;
            }

            elements.Add(value);

            while (this.current_tok.type.Equals(TokenType.COMMA))
            {
                res.register_advancement();
                this.advance();

                value = res.register(this.expr());

                elements.Add(value);
            }

            if (!this.current_tok.type.Equals(TokenType.RBRACE))
            {
                return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '}'"));
            }

            rbrace:  res.register_advancement();
            this.advance();

            res.register_advancement();
            this.advance();

            return res.success(new SetNode(elements, pos_start, this.current_tok.pos_start));
        }
        else if (tok.type.Equals(TokenType.IDENTIFIER))
        {
            res.register_advancement();
            this.advance();

            if (this.current_tok.type.Equals(TokenType.DOT))
            {
                res.register_advancement();
                this.advance();

                if (this.current_tok.type.Equals(TokenType.IDENTIFIER))
                {
                    Token var_name_tok = this.current_tok;
                    res.register_advancement();
                    this.advance();

                    if (this.current_tok.type.Equals(TokenType.LPAREN))
                    {
                        res.register_advancement();
                        this.advance();

                        List<object> arg_nodes = new List<object>();

                        if (this.current_tok.type.Equals(TokenType.RPAREN))
                        {
                            res.register_advancement();
                            this.advance();
                        }
                        else
                        {
                            arg_nodes.Add(res.register(this.expr()));

                            if (res.error != null)
                            {
                                return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ')', ']', 'var', 'if', 'for', 'while', 'fun', int, float, identifier, '+', '-' or '("));
                            }

                            while (this.current_tok.type.Equals(TokenType.COMMA))
                            {
                                res.register_advancement();
                                this.advance();

                                arg_nodes.Add(res.register(this.expr()));

                                if (res.error != null)
                                {
                                    return res;
                                }
                            }

                            if (!this.current_tok.type.Equals(TokenType.RPAREN))
                            {
                                return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ',', ')' or ']'"));
                            }

                            res.register_advancement();
                            this.advance();
                        }

                        return res.success(new ObjectCallNode(tok, var_name_tok, arg_nodes));
                    }

                    if (!this.current_tok.type.Equals(TokenType.EQ) && !this.current_tok.get_string_type().EndsWith("_EQ"))
                    {
                        return res.success(new ObjectAccessNode(tok, var_name_tok));
                    }

                    Token operator_token = this.current_tok;

                    res.register_advancement();
                    this.advance();

                    object expression = res.register(this.expr());

                    if (res.error != null)
                    {
                        return res;
                    }

                    return res.success(new ObjectReAssignNode(tok, var_name_tok, operator_token, expression));
                }
            }
            else if (this.current_tok.type.Equals(TokenType.LSQUARE))
            {
                res.register_advancement();
                this.advance();

                object accessExpr = res.register(this.expr());

                if (res.error != null)
                {
                    return res;
                }

                if (!this.current_tok.type.Equals(TokenType.RSQUARE))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ']'"));
                }

                res.register_advancement();
                this.advance();

                return res.success(new VarListAccessNode(tok, accessExpr));
            }

            if (!this.current_tok.type.Equals(TokenType.EQ) && !this.current_tok.get_string_type().EndsWith("_EQ"))
            {
                return res.success(new VarAccessNode(tok));
            }

            Token op_tok = this.current_tok;

            res.register_advancement();
            this.advance();

            object expr = res.register(this.expr());

            if (res.error != null)
            {
                return res;
            }

            return res.success(new VarReAssignNode(tok, op_tok, expr));
        }
        else if (tok.type.Equals(TokenType.LSQUARE))
        {
            List<object> element_nodes = new List<object>();
            Position pos_start = this.current_tok.pos_start.copy();

            if (!this.current_tok.type.Equals(TokenType.LSQUARE))
            {
                return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '['"));
            }

            res.register_advancement();
            this.advance();

            if (this.current_tok.type.Equals(TokenType.RSQUARE))
            {
                res.register_advancement();
                this.advance();
            }
            else
            {
                element_nodes.Add(res.register(this.expr()));

                if (res.error != null)
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ']', 'var', 'if', 'for', 'while', 'fun', int, float, identifier, '+', '-', '(', '[' or 'not'"));
                }

                while (this.current_tok.type.Equals(TokenType.COMMA))
                {
                    res.register_advancement();
                    this.advance();

                    element_nodes.Add(res.register(this.expr()));

                    if (res.error != null)
                    {
                        return res;
                    }
                }

                if (!this.current_tok.type.Equals(TokenType.RSQUARE))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ',' or ']'"));
                }

                res.register_advancement();
                this.advance();
            }

            return res.success(new ListNode(element_nodes, pos_start, this.current_tok.pos_end.copy()));
        }
        else if (tok.type.Equals(TokenType.LPAREN))
        {
            res.register_advancement();
            this.advance();

            object expr = res.register(this.expr());

            if (res.error != null)
            {
                return res;
            }

            if (this.current_tok.type.Equals(TokenType.RPAREN))
            {
                res.register_advancement();
                this.advance();

                return res.success(expr);
            }
            else
            {
                return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ')'"));
            }
        }
        else if (tok.type.Equals(TokenType.KEYWORD))
        {
            if (tok.value.ToString() == "if")
            {
                Tuple<List<Tuple<object, object, bool>>, Tuple<object, bool>> all_cases = (Tuple<List<Tuple<object, object, bool>>, Tuple<object, bool>>)res.register(this.if_expr_cases("if"));

                if (res.error != null)
                {
                    return res;
                }

                List<Tuple<object, object, bool>> cases = all_cases.Item1;
                Tuple<object, bool> else_case = all_cases.Item2;

                return res.success(new IfNode(cases, else_case));
            }
            else if (tok.value.ToString() == "for")
            {
                if (!this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() != "for")
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected 'for'"));
                }

                res.register_advancement();
                this.advance();

                int lparens = 0;

                while (this.current_tok.type.Equals(TokenType.LPAREN))
                {
                    lparens++;

                    res.register_advancement();
                    this.advance();
                }

                if (!this.current_tok.type.Equals(TokenType.IDENTIFIER))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected identifier"));
                }

                Token var_name = this.current_tok;

                res.register_advancement();
                this.advance();

                if (!this.current_tok.type.Equals(TokenType.EQ))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '='"));
                }

                res.register_advancement();
                this.advance();

                object start_value = res.register(this.expr());

                if (res.error != null)
                {
                    return res;
                }

                if (!this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() != "to")
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected 'to'"));
                }

                res.register_advancement();
                this.advance();

                object end_value = res.register(this.expr());

                if (res.error != null)
                {
                    return res;
                }

                object step_value = null;

                if (this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() == "step")
                {
                    res.register_advancement();
                    this.advance();

                    step_value = res.register(this.expr());

                    if (res.error != null)
                    {
                        return res;
                    }
                }

                for (int i = 0; i < lparens; i++)
                {
                    if (!this.current_tok.type.Equals(TokenType.RPAREN))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ')'"));
                    }

                    res.register_advancement();
                    this.advance();
                }

                while (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    res.register_advancement();
                    this.advance();
                }

                if (!this.current_tok.type.Equals(TokenType.LBRACE) && !this.current_tok.type.Equals(TokenType.COLON))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{' or ':'"));
                }

                Token real_separator = this.current_tok;

                res.register_advancement();
                this.advance();

                if (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    if (!real_separator.type.Equals(TokenType.LBRACE))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{'"));
                    }

                    res.register_advancement();
                    this.advance();

                    object new_body = res.register(this.statements());

                    if (res.error != null)
                    {
                        return res;
                    }

                    if (!this.current_tok.type.Equals(TokenType.RBRACE))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '}'"));
                    }

                    res.register_advancement();
                    this.advance();

                    return res.success(new ForNode(var_name, start_value, end_value, step_value, new_body, true));
                }

                if (!real_separator.type.Equals(TokenType.COLON))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ':'"));
                }

                object body = res.register(this.statement());

                if (res.error != null)
                {
                    return res;
                }

                return res.success(new ForNode(var_name, start_value, end_value, step_value, body, false));
            }
            else if (tok.value.ToString() == "while")
            {
                res.register_advancement();
                this.advance();

                object condition = res.register(this.expr());

                if (res.error != null)
                {
                    return res;
                }

                while (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    res.register_advancement();
                    this.advance();
                }

                if (!this.current_tok.type.Equals(TokenType.LBRACE) && !this.current_tok.type.Equals(TokenType.COLON))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{' or ':'"));
                }

                Token real_separator = this.current_tok;

                res.register_advancement();
                this.advance();

                if (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    if (!real_separator.type.Equals(TokenType.LBRACE))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{'"));
                    }

                    res.register_advancement();
                    this.advance();

                    object new_body = res.register(this.statements());

                    if (res.error != null)
                    {
                        return res;
                    }

                    if (!this.current_tok.type.Equals(TokenType.RBRACE))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '}'"));
                    }

                    res.register_advancement();
                    this.advance();

                    return res.success(new WhileNode(condition, new_body, true));
                }

                if (!real_separator.type.Equals(TokenType.COLON))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ':'"));
                }

                object body = res.register(this.statement());

                if (res.error != null)
                {
                    return res;
                }

                return res.success(new WhileNode(condition, body, false));
            }
            else if (tok.value.ToString() == "fun" || tok.value.ToString() == "async")
            {
                bool async = false;

                if (tok.value.ToString() == "async")
                {
                    res.register_advancement();
                    this.advance();

                    if (this.current_tok.value.ToString() != "fun")
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected 'fun'"));
                    }

                    async = true;
                }

                bool optionalParams = false;

                res.register_advancement();
                this.advance();

                Token var_name_tok = null;

                if (this.current_tok.type.Equals(TokenType.IDENTIFIER))
                {
                    var_name_tok = this.current_tok;

                    res.register_advancement();
                    this.advance();

                    if (!this.current_tok.type.Equals(TokenType.LPAREN))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '('"));
                    }
                }
                else
                {
                    var_name_tok = null;

                    if (!this.current_tok.type.Equals(TokenType.LPAREN))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected identifier or '('"));
                    }
                }

                res.register_advancement();
                this.advance();

                List<Tuple<Token, object>> arg_name_toks = new List<Tuple<Token, object>>();

                if (this.current_tok.type.Equals(TokenType.IDENTIFIER))
                {
                    Token arg_name_tok = this.current_tok;
                    object arg_value = null;

                    res.register_advancement();
                    this.advance();
                    
                    if (this.current_tok.type.Equals(TokenType.EQ))
                    {
                        optionalParams = true;
                        res.register_advancement();
                        this.advance();

                        arg_value = res.register(this.expr());

                        if (res.error != null)
                        {
                            return res;
                        }
                    }
                    else if (optionalParams)
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '='"));
                    }

                    arg_name_toks.Add(new Tuple<Token, object>(arg_name_tok, arg_value));

                    while (this.current_tok.type.Equals(TokenType.COMMA))
                    {
                        res.register_advancement();
                        this.advance();

                        if (!this.current_tok.type.Equals(TokenType.IDENTIFIER))
                        {
                            return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected identifier"));
                        }

                        Token arg_tok = this.current_tok;
                        object arg_val = null;

                        res.register_advancement();
                        this.advance();

                        if (this.current_tok.type.Equals(TokenType.EQ))
                        {
                            optionalParams = true;
                            res.register_advancement();
                            this.advance();

                            arg_val = res.register(this.expr());

                            if (res.error != null)
                            {
                                return res;
                            }
                        }
                        else if (optionalParams)
                        {
                            return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '='"));
                        }

                        arg_name_toks.Add(new Tuple<Token, object>(arg_tok, arg_val));
                    }

                    if (!this.current_tok.type.Equals(TokenType.RPAREN))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ',' or ')'"));
                    }
                }
                else
                {
                    if (!this.current_tok.type.Equals(TokenType.RPAREN))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected identifier or ')'"));
                    }
                }

                res.register_advancement();
                this.advance();

                if (this.current_tok.type.Equals(TokenType.ARROW))
                {
                    res.register_advancement();
                    this.advance();

                    object node_to_return = res.register(this.expr());

                    if (res.error != null)
                    {
                        return res;
                    }

                    return res.success(new FuncDefNode(var_name_tok, arg_name_toks, node_to_return, true, async));
                }

                while (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    res.register_advancement();
                    this.advance();
                }

                if (!this.current_tok.type.Equals(TokenType.LBRACE))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{'"));
                }

                res.register_advancement();
                this.advance();

                if (!this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '->' or a new line"));
                }

                res.register_advancement();
                this.advance();

                object body = res.register(this.statements());

                if (!this.current_tok.type.Equals(TokenType.RBRACE))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '}'"));
                }

                res.register_advancement();
                this.advance();

                return res.success(new FuncDefNode(var_name_tok, arg_name_toks, body, false, async));
            }
            else if (tok.value.ToString() == "do")
            {
                res.register_advancement();
                this.advance();

                while (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    res.register_advancement();
                    this.advance();
                }

                if (!this.current_tok.type.Equals(TokenType.LBRACE) && !this.current_tok.type.Equals(TokenType.COLON))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{' or ':'"));
                }

                Token real_separator = this.current_tok;

                res.register_advancement();
                this.advance();

                if (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    if (!real_separator.type.Equals(TokenType.LBRACE))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{'"));
                    }

                    res.register_advancement();
                    this.advance();

                    object new_body = res.register(this.statements());

                    if (res.error != null)
                    {
                        return res;
                    }

                    if (!this.current_tok.type.Equals(TokenType.RBRACE))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '}'"));
                    }

                    res.register_advancement();
                    this.advance();

                    while (this.current_tok.type.Equals(TokenType.NEWLINE))
                    {
                        res.register_advancement();
                        this.advance();
                    }

                    if (this.current_tok.type.Equals(TokenType.KEYWORD))
                    {
                        if (this.current_tok.value.ToString() != "while")
                        {
                            return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected 'while'"));
                        }

                        res.register_advancement();
                        this.advance();

                        object expr = res.register(this.expr());

                        if (res.error != null)
                        {
                            return res;
                        }

                        return res.success(new DoWhileNode(expr, new_body, true));
                    }
                    else
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected 'while'"));
                    }
                }

                if (!real_separator.type.Equals(TokenType.COLON))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ':'"));
                }

                object body = res.register(this.statement());

                if (res.error != null)
                {
                    return res;
                }

                if (this.current_tok.type.Equals(TokenType.KEYWORD))
                {
                    if (this.current_tok.value.ToString() != "while")
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected 'while'"));
                    }

                    res.register_advancement();
                    this.advance();

                    object expr = res.register(this.expr());

                    if (res.error != null)
                    {
                        return res;
                    }

                    return res.success(new DoWhileNode(expr, body, false));
                }

                return res.success(null);
            }
            else if (tok.value.ToString() == "foreach")
            {
                res.register_advancement();
                this.advance();

                int lparens = 0;

                while (this.current_tok.type.Equals(TokenType.LPAREN))
                {
                    lparens++;

                    res.register_advancement();
                    this.advance();
                }

                if (!this.current_tok.type.Equals(TokenType.IDENTIFIER))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected identifier"));
                }

                Token element_var_name = this.current_tok;

                res.register_advancement();
                this.advance();

                if (!this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() != "in")
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected 'in'"));
                }

                res.register_advancement();
                this.advance();

                object expr = res.register(this.expr());

                if (res.error != null)
                {
                    return res;
                }

                for (int i = 0; i < lparens; i++)
                {
                    if (!this.current_tok.type.Equals(TokenType.RPAREN))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ')'"));
                    }

                    res.register_advancement();
                    this.advance();
                }

                while (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    res.register_advancement();
                    this.advance();
                }

                if (!this.current_tok.type.Equals(TokenType.LBRACE) && !this.current_tok.type.Equals(TokenType.COLON))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{' or ':'"));
                }

                Token real_separator = this.current_tok;

                res.register_advancement();
                this.advance();

                if (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    if (!real_separator.type.Equals(TokenType.LBRACE))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{'"));
                    }

                    res.register_advancement();
                    this.advance();

                    object new_body = res.register(this.statements());

                    if (res.error != null)
                    {
                        return res;
                    }

                    if (!this.current_tok.type.Equals(TokenType.RBRACE))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '}'"));
                    }

                    res.register_advancement();
                    this.advance();

                    return res.success(new ForEachNode(element_var_name, expr, new_body, true));
                }

                if (!real_separator.type.Equals(TokenType.COLON))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ':'"));
                }

                object body = res.register(this.statement());

                if (res.error != null)
                {
                    return res;
                }

                return res.success(new ForEachNode(element_var_name, expr, body, false));
            }
            else if (tok.value.ToString() == "switch")
            {
                List<Tuple<object, object>> cases = new List<Tuple<object, object>>();
                object default_case = null;

                res.register_advancement();
                this.advance();

                int lparens = 0;

                while (this.current_tok.type.Equals(TokenType.LPAREN))
                {
                    lparens++;

                    res.register_advancement();
                    this.advance();
                }

                object comparisonExpr = res.register(this.expr());

                if (res.error != null)
                {
                    return res;
                }

                for (int i = 0; i < lparens; i++)
                {
                    if (!this.current_tok.type.Equals(TokenType.RPAREN))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ')'"));
                    }

                    res.register_advancement();
                    this.advance();
                }

                while (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    res.register_advancement();
                    this.advance();
                }

                if (!this.current_tok.type.Equals(TokenType.LBRACE) && !this.current_tok.type.Equals(TokenType.COLON))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{'"));
                }

                res.register_advancement();
                this.advance();

                if (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    res.register_advancement();
                    this.advance();

                    while (this.current_tok.type.Equals(TokenType.NEWLINE))
                    {
                        res.register_advancement();
                        this.advance();
                    }

                    while (this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() == "case")
                    {
                        res.register_advancement();
                        this.advance();

                        object expr = res.register(this.expr());

                        if (res.error != null)
                        {
                            return res;
                        }

                        if (!this.current_tok.type.Equals(TokenType.COLON))
                        {
                            return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ':'"));
                        }

                        res.register_advancement();
                        this.advance();

                        object statements = res.register(this.statements());

                        if (res.error != null)
                        {
                            return res;
                        }

                        cases.Add(new Tuple<object, object>(expr, statements));
                    }

                    if (this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() == "default")
                    {
                        res.register_advancement();
                        this.advance();

                        if (!this.current_tok.type.Equals(TokenType.COLON))
                        {
                            return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ':'"));
                        }

                        res.register_advancement();
                        this.advance();

                        object statements = res.register(this.statements());

                        if (res.error != null)
                        {
                            return res;
                        }

                        default_case = statements;
                    }

                    while (this.current_tok.type.Equals(TokenType.NEWLINE))
                    {
                        res.register_advancement();
                        this.advance();
                    }

                    if (!this.current_tok.type.Equals(TokenType.RBRACE))
                    {
                        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '}'"));
                    }

                    res.register_advancement();
                    this.advance();
                }
                else
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected new line"));
                }

                return res.success(new SwitchNode(comparisonExpr, cases, default_case));
            }
            else if (tok.value.ToString() == "struct")
            {
                res.register_advancement();
                this.advance();

                if (!this.current_tok.type.Equals(TokenType.IDENTIFIER))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected identifier"));
                }

                Token var_name_tok = this.current_tok;

                res.register_advancement();
                this.advance();

                while (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    res.register_advancement();
                    this.advance();
                }

                if (!this.current_tok.type.Equals(TokenType.LBRACE))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{'"));
                }

                res.register_advancement();
                this.advance();

                object statements = res.register(this.statements());

                if (res.error != null)
                {
                    return res;
                }

                while (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    res.register_advancement();
                    this.advance();
                }

                if (!this.current_tok.type.Equals(TokenType.RBRACE))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '}'"));
                }

                res.register_advancement();
                this.advance();

                return res.success(new StructDefNode(var_name_tok, statements));
            }
            else if (tok.value.ToString() == "del")
            {
                res.register_advancement();
                this.advance();

                if (this.current_tok.type.Equals(TokenType.IDENTIFIER))
                {
                    Token theTok = this.current_tok;

                    res.register_advancement();
                    this.advance();

                    return res.success(new DeleteNode(theTok));
                }

                return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected identifier"));
            }
            else if (tok.value.ToString() == "goto")
            {
                res.register_advancement();
                this.advance();

                if (this.current_tok.type.Equals(TokenType.IDENTIFIER))
                {
                    Token var_name_tok = this.current_tok;

                    res.register_advancement();
                    this.advance();

                    return res.success(new GotoNode(var_name_tok));
                }
            }
            else if (tok.value.ToString() == "import")
            {
                res.register_advancement();
                this.advance();

                if (this.current_tok.type.Equals(TokenType.STRING))
                {
                    Token string_tok = this.current_tok;

                    res.register_advancement();
                    this.advance();

                    return res.success(new ImportNode(string_tok));
                }
            }
            else if (tok.value.ToString() == "use")
            {
                res.register_advancement();
                this.advance();

                if (this.current_tok.type.Equals(TokenType.IDENTIFIER))
                {
                    Token var_name_tok = this.current_tok;

                    res.register_advancement();
                    this.advance();

                    return res.success(new UseNode(var_name_tok));
                }
            }
            else if (tok.value.ToString() == "namespace")
            {
                res.register_advancement();
                this.advance();

                if (!this.current_tok.type.Equals(TokenType.IDENTIFIER))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected identifier"));
                }

                Token var_name_tok = this.current_tok;

                res.register_advancement();
                this.advance();

                while (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    res.register_advancement();
                    this.advance();
                }

                if (!this.current_tok.type.Equals(TokenType.LBRACE))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{'"));
                }

                res.register_advancement();
                this.advance();

                object statements = res.register(this.statements());

                if (res.error != null)
                {
                    return res;
                }

                while (this.current_tok.type.Equals(TokenType.NEWLINE))
                {
                    res.register_advancement();
                    this.advance();
                }

                if (!this.current_tok.type.Equals(TokenType.RBRACE))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '}'"));
                }

                res.register_advancement();
                this.advance();

                return res.success(new NamespaceDefNode(var_name_tok, statements));
            }
        }

        return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected int, float, identifier, '+', '-', '(', '[', 'if', 'for', 'while' or 'fun'"));
    }

    public ParseResult power()
    {
        return bin_op("call", TokenType.POW, TokenType.MODULO, "factor");
    }

    public ParseResult call()
    {
        ParseResult res = new ParseResult();

        object atom = res.register(this.atom());

        if (res.error != null)
        {
            return res;
        }

        if (this.current_tok.type.Equals(TokenType.LPAREN))
        {
            res.register_advancement();
            this.advance();

            List<object> arg_nodes = new List<object>();

            if (this.current_tok.type.Equals(TokenType.RPAREN))
            {
                res.register_advancement();
                this.advance();
            }
            else
            {
                arg_nodes.Add(res.register(this.expr()));

                if (res.error != null)
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ')', ']', 'var', 'if', 'for', 'while', 'fun', int, float, identifier, '+', '-' or '("));
                }

                while (this.current_tok.type.Equals(TokenType.COMMA))
                {
                    res.register_advancement();
                    this.advance();

                    arg_nodes.Add(res.register(this.expr()));

                    if (res.error != null)
                    {
                        return res;
                    }
                }

                if (!this.current_tok.type.Equals(TokenType.RPAREN))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected ',', ')' or ']'"));
                }

                res.register_advancement();
                this.advance();
            }

            return res.success(new CallNode(atom, arg_nodes));
        }

        return res.success(atom);
    }

    public ParseResult factor()
    {
        ParseResult res = new ParseResult();
        Token tok = this.current_tok;

        if (tok.type.Equals(TokenType.PLUS) || tok.type.Equals(TokenType.MINUS))
        {
            res.register_advancement();
            this.advance();

            object factor = res.register(this.factor());

            if (res.error != null)
            {
                return res;
            }

            return res.success(new UnaryOpNode(tok, factor));
        }

        return this.power();
    }

    public ParseResult term()
    {
        return this.bin_op("factor", TokenType.MUL, TokenType.DIV);
    }

    public ParseResult comp_expr()
    {
        ParseResult res = new ParseResult();

        if (this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() == "not")
        {
            Token op_tok = this.current_tok;

            res.register_advancement();
            this.advance();

            object node = res.register(this.comp_expr());

            if (res.error != null)
            {
                return res;
            }

            return res.success(new UnaryOpNode(op_tok, node));
        }
        else if (this.current_tok.type.Equals(TokenType.LOGIC_NOT))
        {
            Token op_tok = this.current_tok;

            res.register_advancement();
            this.advance();

            object node = res.register(this.expr());

            if (res.error != null)
            {
                return res;
            }

            return res.success(new UnaryOpNode(op_tok, node));
        }

        object node1 = res.register(this.bin_op2("arith_expr"));

        if (res.error != null)
        {
            return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected int, float, identifier, '+', '-', '(', '[' or 'not'"));
        }

        return res.success(node1);
    }

    public ParseResult arith_expr()
    {
        return this.bin_op("term", TokenType.PLUS, TokenType.MINUS);
    }

    public ParseResult expr()
    {
        ParseResult res = new ParseResult();
        bool beConstant = false;

        if (this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() == "const")
        {
            beConstant = true;
            res.register_advancement();
            this.advance();
        }

        if (beConstant && !(this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() == "var"))
        {
            return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected 'var'"));
        }

        if (this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() == "var")
        {
            List<object> variables = new List<object>();

            res.register_advancement();
            this.advance();

            if (!this.current_tok.type.Equals(TokenType.IDENTIFIER))
            {
                return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected identifier"));
            }

            Token var_name = this.current_tok;

            res.register_advancement();
            this.advance();

            if (!this.current_tok.type.Equals(TokenType.EQ))
            {
                variables.Add(new VarAssignNode(var_name, new NumberNode(new Token(TokenType.INT, (int)0, var_name.pos_start, var_name.pos_end)), !beConstant));
            }
            else
            {
                res.register_advancement();
                this.advance();

                object expr = res.register(this.expr());

                if (res.error != null)
                {
                    return res;
                }

                variables.Add(new VarAssignNode(var_name, expr, !beConstant));
            }

            while (this.current_tok.type.Equals(TokenType.COMMA))
            {
                res.register_advancement();
                this.advance();

                if (!this.current_tok.type.Equals(TokenType.IDENTIFIER))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected identifier"));
                }

                Token new_var_name = this.current_tok;

                res.register_advancement();
                this.advance();

                if (!this.current_tok.type.Equals(TokenType.EQ))
                {
                    variables.Add(new VarAssignNode(new_var_name, new NumberNode(new Token(TokenType.INT, (int)0, new_var_name.pos_start, new_var_name.pos_end)), !beConstant));
                    continue;
                }

                res.register_advancement();
                this.advance();

                object new_expr = res.register(this.expr());

                if (res.error != null)
                {
                    return res;
                }

                variables.Add(new VarAssignNode(new_var_name, new_expr, !beConstant));
            }

            return res.success(new ListNode(variables, var_name.pos_start, this.current_tok.pos_start));
        }

        object node = res.register(this.bin_op1("comp_expr", "and", "or"));

        if (res.error != null)
        {
            return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected 'var', 'if', 'for', 'while', 'fun', int, float, identifier, '+', '-', '(', '[' or 'not'"));
        }

        return res.success(node);
    }

    public ParseResult bin_op(string func_a, TokenType op1, TokenType op2, string func_b = null)
    {
        if (func_b == null)
        {
            func_b = func_a;
        }

        ParseResult res = new ParseResult();
        object left = res.register((ParseResult)this.GetType().GetMethod(func_a).Invoke(this, new object[] { }));

        if (res.error != null)
        {
            return res;
        }

        while (this.current_tok.type == op1 || this.current_tok.type == op2)
        {
            Token op_tok = this.current_tok;

            res.register_advancement();
            this.advance();

            object right = res.register((ParseResult)this.GetType().GetMethod(func_b).Invoke(this, new object[] { }));

            if (res.error != null)
            {
                return res;
            }

            left = new BinOpNode(left, op_tok, right);
        }

        return res.success(left);
    }

    public ParseResult bin_op1(string func_a, string op1, string op2, string func_b = null)
    {
        if (func_b == null)
        {
            func_b = func_a;
        }

        ParseResult res = new ParseResult();
        object left = res.register((ParseResult)this.GetType().GetMethod(func_a).Invoke(this, new object[] { }));

        if (res.error != null)
        {
            return res;
        }

        while (this.current_tok.type.Equals(TokenType.KEYWORD) && (this.current_tok.value.ToString() == op1 || this.current_tok.value.ToString() == op2))
        {
            Token op_tok = this.current_tok;

            res.register_advancement();
            this.advance();

            object right = res.register((ParseResult)this.GetType().GetMethod(func_b).Invoke(this, new object[] { }));

            if (res.error != null)
            {
                return res;
            }

            left = new BinOpNode(left, op_tok, right);
        }

        while (this.current_tok.type.Equals(TokenType.LOGIC_AND) || this.current_tok.type.Equals(TokenType.LOGIC_OR))
        {
            Token op_tok = this.current_tok;

            res.register_advancement();
            this.advance();

            object right = res.register((ParseResult)this.GetType().GetMethod(func_b).Invoke(this, new object[] { }));

            if (res.error != null)
            {
                return res;
            }

            left = new BinOpNode(left, op_tok, right);
        }

        while (this.current_tok.type.Equals(TokenType.LEFT_SHIFT) || this.current_tok.type.Equals(TokenType.RIGHT_SHIFT))
        {
            Token op_tok = this.current_tok;

            res.register_advancement();
            this.advance();

            object right = res.register((ParseResult)this.GetType().GetMethod(func_b).Invoke(this, new object[] { }));

            if (res.error != null)
            {
                return res;
            }

            left = new BinOpNode(left, op_tok, right);
        }

        return res.success(left);
    }

    public ParseResult bin_op2(string func_a, string func_b = null)
    {
        if (func_b == null)
        {
            func_b = func_a;
        }

        ParseResult res = new ParseResult();
        object left = res.register((ParseResult)this.GetType().GetMethod(func_a).Invoke(this, new object[] { }));

        if (res.error != null)
        {
            return res;
        }

        while (this.current_tok.type.Equals(TokenType.EE) || this.current_tok.type.Equals(TokenType.NE) || this.current_tok.type.Equals(TokenType.LT) || this.current_tok.type.Equals(TokenType.GT) || this.current_tok.type.Equals(TokenType.LTE) || this.current_tok.type.Equals(TokenType.GTE) || (this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() == "in"))
        {
            Token op_tok = this.current_tok;

            res.register_advancement();
            this.advance();

            object right = res.register((ParseResult)this.GetType().GetMethod(func_b).Invoke(this, new object[] { }));

            if (res.error != null)
            {
                return res;
            }

            left = new BinOpNode(left, op_tok, right);
        }

        return res.success(left);
    }

    public ParseResult if_expr_cases(string case_keyword)
    {
        ParseResult res = new ParseResult();
        List<Tuple<object, object, bool>> cases = new List<Tuple<object, object, bool>>();
        Tuple<object, bool> else_case = null;

        if (!this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() != case_keyword)
        {
            return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '" + case_keyword + "'"));
        }

        res.register_advancement();
        this.advance();

        object condition = res.register(this.expr());

        if (res.error != null)
        {
            return res;
        }

        while (this.current_tok.type.Equals(TokenType.NEWLINE))
        {
            res.register_advancement();
            this.advance();
        }

        if (!this.current_tok.type.Equals(TokenType.LBRACE) && !this.current_tok.type.Equals(TokenType.COLON))
        {
            return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{' or ':'"));
        }

        Token separator_token = this.current_tok;

        res.register_advancement();
        this.advance();

        if (this.current_tok.type.Equals(TokenType.NEWLINE))
        {
            if (!separator_token.type.Equals(TokenType.LBRACE))
            {
                return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{'"));
            }

            res.register_advancement();
            this.advance();

            object statements = res.register(this.statements());

            if (res.error != null)
            {
                return res;
            }

            cases.Add(new Tuple<object, object, bool>(condition, statements, true));

            if (this.current_tok.type.Equals(TokenType.RBRACE))
            {
                res.register_advancement();
                this.advance();
            }

            int to_reverse_count = 0;

            while (this.current_tok.type.Equals(TokenType.NEWLINE))
            {
                to_reverse_count++;

                res.register_advancement();
                this.advance();
            }

            if (this.current_tok.type.Equals(TokenType.KEYWORD) && (this.current_tok.value.ToString() == "else" || this.current_tok.value.ToString() == "elif"))
            {
                Tuple<List<Tuple<object, object, bool>>, Tuple<object, bool>> all_cases = (Tuple<List<Tuple<object, object, bool>>, Tuple<object, bool>>)res.register(this.if_expr_b_or_c());

                if (res.error != null)
                {
                    return res;
                }

                List<Tuple<object, object, bool>> new_cases = all_cases.Item1;

                else_case = all_cases.Item2;
                cases.AddRange(new_cases);
            }
            else
            {
                this.tok_idx -= to_reverse_count;
                this.update_current_tok();
            }
        }
        else
        {
            object expr = res.register(this.statement());

            if (res.error != null)
            {
                return res;
            }

            cases.Add(new Tuple<object, object, bool>(condition, expr, false));
            Tuple<List<Tuple<object, object, bool>>, Tuple<object, bool>> all_cases = (Tuple<List<Tuple<object, object, bool>>, Tuple<object, bool>>)res.register(this.if_expr_b_or_c());

            if (res.error != null)
            {
                return res;
            }

            List<Tuple<object, object, bool>> new_cases = all_cases.Item1;

            else_case = all_cases.Item2;
            cases.AddRange(new_cases);
        }

        return res.success(new Tuple<List<Tuple<object, object, bool>>, Tuple<object, bool>>(cases, else_case));
    }

    public ParseResult if_expr_b()
    {
        return this.if_expr_cases("elif");
    }

    public ParseResult if_expr_c()
    {
        ParseResult res = new ParseResult();
        Tuple<object, bool> else_case = null;

        if (this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() == "else")
        {
            res.register_advancement();
            this.advance();

            while (this.current_tok.type.Equals(TokenType.NEWLINE))
            {
                res.register_advancement();
                this.advance();
            }

            if (!this.current_tok.type.Equals(TokenType.LBRACE) && !this.current_tok.type.Equals(TokenType.COLON))
            {
                return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{' or ':'"));
            }

            Token separator_token = this.current_tok;

            res.register_advancement();
            this.advance();

            if (this.current_tok.type.Equals(TokenType.NEWLINE))
            {
                if (!separator_token.type.Equals(TokenType.LBRACE))
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '{'"));
                }

                res.register_advancement();
                this.advance();

                object statements = res.register(this.statements());

                if (res.error != null)
                {
                    return res;
                }

                else_case = new Tuple<object, bool>(statements, true);

                if (this.current_tok.type.Equals(TokenType.RBRACE))
                {
                    res.register_advancement();
                    this.advance();
                }
                else
                {
                    return res.failure(new InvalidSyntaxError(this.current_tok.pos_start, this.current_tok.pos_end, "Expected '}'"));
                }
            }
            else
            {
                object expr = res.register(this.statement());

                if (res.error != null)
                {
                    return res;
                }

                else_case = new Tuple<object, bool>(expr, false);
            }
        }

        return res.success(else_case);
    }

    public ParseResult if_expr_b_or_c()
    {
        ParseResult res = new ParseResult();
        List<Tuple<object, object, bool>> cases = new List<Tuple<object, object, bool>>();
        Tuple<object, bool> else_case = null;

        if (this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() == "elif")
        {
            Tuple<List<Tuple<object, object, bool>>, Tuple<object, bool>> all_cases = (Tuple<List<Tuple<object, object, bool>>, Tuple<object, bool>>)res.register(this.if_expr_b());

            if (res.error != null)
            {
                return res;
            }

            cases = all_cases.Item1;
            else_case = all_cases.Item2;
        }
        else
        {
            else_case = (Tuple<object, bool>)res.register(this.if_expr_c());

            if (res.error != null)
            {
                return res;
            }
        }

        return res.success(new Tuple<List<Tuple<object, object, bool>>, Tuple<object, bool>>(cases, else_case));
    }

    public ParseResult statements()
    {
        ParseResult res = new ParseResult();
        List<object> statements = new List<object>();
        Position pos_start = this.current_tok.pos_start.copy();

        while (this.current_tok.type.Equals(TokenType.NEWLINE))
        {
            res.register_advancement();
            this.advance();
        }

        object statement = res.register(this.statement());

        if (res.error != null)
        {
            return res;
        }

        statements.Add(statement);
        bool more_statements = true;

        while (true)
        {
            int newline_count = 0;

            while (this.current_tok.type.Equals(TokenType.NEWLINE))
            {
                res.register_advancement();
                this.advance();

                newline_count++;
            }

            if (newline_count == 0)
            {
                more_statements = false;
            }

            if (!more_statements)
            {
                break;
            }

            statement = res.try_register(this.statement());

            if (statement == null)
            {
                this.reverse(res.to_reverse_count);
                more_statements = false;
                continue;
            }

            statements.Add(statement);
        }

        return res.success(new ListNode(statements, pos_start, this.current_tok.pos_end.copy()));
    }

    public ParseResult statement()
    {
        ParseResult res = new ParseResult();
        Position pos_start = this.current_tok.pos_start.copy();

        if (this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() == "return")
        {
            res.register_advancement();
            this.advance();

            object theExpr = res.try_register(this.expr());

            if (theExpr == null)
            {
                this.reverse(res.to_reverse_count);
            }

            return res.success(new ReturnNode(theExpr, pos_start, this.current_tok.pos_start.copy()));
        }

        if (this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() == "continue")
        {
            res.register_advancement();
            this.advance();

            return res.success(new ContinueNode(pos_start, this.current_tok.pos_start.copy()));
        }

        if (this.current_tok.type.Equals(TokenType.KEYWORD) && this.current_tok.value.ToString() == "break")
        {
            res.register_advancement();
            this.advance();

            return res.success(new BreakNode(pos_start, this.current_tok.pos_start.copy()));
        }

        if (this.current_tok.type.Equals(TokenType.IDENTIFIER))
        {
            if (this.tokens[this.tok_idx + 1].type.Equals(TokenType.DOUBLE_PLUS) || this.tokens[this.tok_idx + 1].type.Equals(TokenType.DOUBLE_MINUS))
            {
                Token var_name_tok = this.current_tok;

                res.register_advancement();
                this.advance();

                Token op_tok = this.current_tok;

                res.register_advancement();
                this.advance();

                return res.success(new VarReAssignNode(var_name_tok, op_tok, null));
            }
            else if (this.tokens[this.tok_idx + 1].type.Equals(TokenType.COLON))
            {
                Token var_name_tok = this.current_tok;

                res.register_advancement();
                this.advance();

                res.register_advancement();
                this.advance();

                object statements = res.register(this.statements());

                if (res.error != null)
                {
                    return res;
                }

                return res.success(new LabelNode(var_name_tok, statements));
            }
        }

        object expr = res.register(this.expr());

        if (res.error != null)
        {
            res.error = null;
            return res.success(new NumberNode(new Token(TokenType.INT, 0, this.current_tok.pos_start, this.current_tok.pos_end)));
        }

        return res.success(expr);
    }
}