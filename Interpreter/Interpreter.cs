using System;
using System.Collections.Generic;

public class Interpreter
{
    public RuntimeResult visit(object node, Context context)
    {
        return (RuntimeResult)this.GetType().GetMethod("visit_" + node.GetType().Name).Invoke(this, new object[] { node, context });
    }

    public RuntimeResult visit_NumberNode(NumberNode node, Context context)
    {
        return new RuntimeResult().success(new NumberValue(node.tok.value).set_context(context).set_pos(node.pos_start, node.pos_end));
    }

    public RuntimeResult visit_BinOpNode(BinOpNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();
        object left = res.register(this.visit(node.left_node, context));

        if (res.should_return())
        {
            return res;
        }

        object right = res.register(this.visit(node.right_node, context));

        if (res.should_return())
        {
            return res;
        }

        Tuple<object, Error> result = null;

        if (node.op_tok.type == "KEYWORD")
        {
            result = (Tuple<object, Error>)left.GetType().GetMethod(node.op_tok.value.ToString().ToLower() + "ed_by").Invoke(left, new object[] { right });
        }
        else if (left.GetType().GetMethod("get_comparison_" + node.op_tok.type.ToLower()) != null)
        {
            result = (Tuple<object, Error>)left.GetType().GetMethod("get_comparison_" + node.op_tok.type.ToLower()).Invoke(left, new object[] { right });
        }
        else
        {
            result = (Tuple<object, Error>)left.GetType().GetMethod(node.op_tok.type.ToLower() + "ed_by").Invoke(left, new object[] { right });
        }

        if (result.Item2 != null)
        {
            return res.failure(result.Item2);
        }

        return res.success(result.Item1.GetType().GetMethod("set_pos").Invoke(result.Item1, new object[] { node.pos_start, node.pos_end }));
    }

    public RuntimeResult visit_UnaryOpNode(UnaryOpNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();
        object number = res.register(this.visit(node.node, context));

        if (res.should_return())
        {
            return res;
        }

        Error error = null;

        if (node.op_tok.type == "MINUS")
        {
            number = (Tuple<object, Error>) number.GetType().GetMethod("muled_by").Invoke(number, new object[] { new NumberValue(-1) });
        }
        else if (node.op_tok.type == "PLUS")
        {
            number = (Tuple<object, Error>)number.GetType().GetMethod("muled_by").Invoke(number, new object[] { new NumberValue(1) });
        }
        else if (node.op_tok.type == "KEYWORD" && node.op_tok.value.ToString() == "not")
        {
            number = (Tuple<object, Error>)number.GetType().GetMethod("notted").Invoke(number, new object[] { });
        }
        else if (node.op_tok.type == "LOGIC_NOT")
        {
            number = (Tuple<object, Error>)number.GetType().GetMethod("logic_notted").Invoke(number, new object[] { });
        }

        error = (Error)number.GetType().GetProperty("Item2").GetValue(number);

        if (error != null)
        {
            return res.failure(error);
        }

        object realNumber = number.GetType().GetProperty("Item1").GetValue(number);
        return res.success(realNumber.GetType().GetMethod("set_pos").Invoke(realNumber, new object[] { node.pos_start, node.pos_end }));
    }

    public RuntimeResult visit_VarAccessNode(VarAccessNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        if (context.symbol_table.get(node.var_name_tok.value) == null)
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "'" + node.var_name_tok.value.ToString() + "' is not defined", context));
        }

        return res.success(context.symbol_table.get(node.var_name_tok.value));
    }

    public RuntimeResult visit_VarAssignNode(VarAssignNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        object var_name = node.var_name_tok.value;
        object value = res.register(this.visit(node.value_node, context));

        if (res.should_return())
        {
            return res;
        }

        if (value.GetType() == typeof(StructValue))
        {
            if (!((StructValue) value).already_declared)
            {
                value = new StructValue(((StructValue)value).name, ((StructValue)value).statements).set_context(context).set_pos(node.pos_start, node.pos_end).declare(this);
            }
        }
        else if (value.GetType() == typeof(BuiltInStruct))
        {
            if (!((BuiltInStruct) value).already_declared)
            {
                value = new BuiltInStruct(((BuiltInStruct)value).name).set_context(context).set_pos(node.pos_start, node.pos_end).declare();
            }
        }

        if (context.symbol_table.can_be_rewrite(var_name))
        {
            context.symbol_table.set(var_name, value, node.rewritten);
        }
        else
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Can not access to constants variables", context));
        }
        
        if (value.GetType() != typeof(BuiltInStruct))
        {
            value = value.GetType().GetMethod("copy").Invoke(value, new object[] { });
            value.GetType().GetMethod("set_pos").Invoke(value, new object[] { node.pos_start, node.pos_end });
        }

        return res.success(value);
    }

    public RuntimeResult visit_VarReAssignNode(VarReAssignNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        object var_name = node.var_name_tok.value;
        object value = null;

        if (node.value_node != null)
        {
            value = res.register(this.visit(node.value_node, context));

            if (res.should_return())
            {
                return res;
            }
        }

        if (context.symbol_table.present(var_name))
        {
            if (!context.symbol_table.can_be_rewrite(var_name))
            {
                return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Can not access to constants variables", context));
            }

            object actualValue = context.symbol_table.get(var_name);

            if (node.op_tok.type == "EQ")
            {
                context.symbol_table.set(var_name, value);
            }
            else if (node.op_tok.type.StartsWith("DOUBLE"))
            {
                Tuple<object, Error> result = (Tuple<object, Error>)actualValue.GetType().GetMethod(node.op_tok.type.ToLower().Replace("double_", "") + "ed_by").Invoke(actualValue, new object[] { new NumberValue(1) });
                value = result.Item1;
                context.symbol_table.set(var_name, value);
            }
            else
            {
                Tuple<object, Error> result = (Tuple<object, Error>)actualValue.GetType().GetMethod(node.op_tok.type.ToLower().Replace("_eq", "ed_by")).Invoke(actualValue, new object[] { value });
                value = result.Item1;
                context.symbol_table.set(var_name, value);
            }

            value = value.GetType().GetMethod("copy").Invoke(value, new object[] { });
            value.GetType().GetMethod("set_pos").Invoke(value, new object[] { node.pos_start, node.pos_end });

            return res.success(value);
        }

        return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Variable is not created", context));
    }
    public RuntimeResult visit_DeleteNode(DeleteNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        object var_name = node.var_name_tok.value;

        if (context.symbol_table.present(var_name))
        {
            context.symbol_table.remove(var_name);

            return res.success(Values.NULL);
        }

        return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Variable is not created", context));
    }

    public RuntimeResult visit_IfNode(IfNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        foreach (Tuple<object, object, bool> conditionExpr in node.cases)
        {
            object condition_value = res.register(this.visit(conditionExpr.Item1, context));

            if (res.should_return())
            {
                return res;
            }

            if ((bool)condition_value.GetType().GetMethod("is_true").Invoke(condition_value, new object[] { }))
            {
                object expr_value = res.register(this.visit(conditionExpr.Item2, context));

                if (res.should_return())
                {
                    return res;
                }

                return res.success(conditionExpr.Item3 ? Values.NULL : expr_value);
            }
        }

        if (node.else_case != null)
        {
            object else_value = res.register(this.visit(node.else_case.Item1, context));

            if (res.should_return())
            {
                return res;
            }

            return res.success(node.else_case.Item2 ? Values.NULL : else_value);
        }

        return res.success(Values.NULL);
    }

    public RuntimeResult visit_ForNode(ForNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();
        NumberValue start_value = (NumberValue) res.register(this.visit(node.start_value_node, context));

        if (res.should_return())
        {
            return res;
        }

        NumberValue end_value = (NumberValue)res.register(this.visit(node.end_value_node, context));

        if (res.should_return())
        {
            return res;
        }

        NumberValue step_value = new NumberValue(1);

        if (node.step_value_node != null)
        {
            step_value = (NumberValue) res.register(this.visit(node.step_value_node, context));
        }

        int i = (int) start_value.value;

        if ((int) step_value.value >= 0)
        {
            while (i < (int) end_value.value)
            {
                context.symbol_table.set(node.var_name_tok.value, new NumberValue(i));
                i += (int) step_value.value;
                object value = res.register(this.visit(node.body_node, context));

                if (res.should_return() && !res.loop_should_continue && !res.loop_should_break)
                {
                    return res;
                }

                if (res.loop_should_continue)
                {
                    continue;
                }

                if (res.loop_should_break)
                {
                    break;
                }
            }
        }    
        else
        {
            while (i > (int)end_value.value)
            {
                context.symbol_table.set(node.var_name_tok.value, new NumberValue(i));
                i += (int)step_value.value;
                object value = res.register(this.visit(node.body_node, context));

                if (res.should_return() && !res.loop_should_continue && !res.loop_should_break)
                {
                    return res;
                }

                if (res.loop_should_continue)
                {
                    continue;
                }

                if (res.loop_should_break)
                {
                    break;
                }
            }
        }

        return res.success(Values.NULL);
    }

    public RuntimeResult visit_WhileNode(WhileNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        while (true)
        {
            NumberValue condition = (NumberValue) res.register(this.visit(node.condition_node, context));

            if (res.should_return())
            {
                return res;
            }

            if (!condition.is_true())
            {
                break;
            }

            object value = res.register(this.visit(node.body_node, context));

            if (res.should_return() && !res.loop_should_continue && !res.loop_should_break)
            {
                return res;
            }

            if (res.loop_should_continue)
            {
                continue;
            }

            if (res.loop_should_break)
            {
                break;
            }
        }

        return res.success(Values.NULL);
    }

    public RuntimeResult visit_DoWhileNode(DoWhileNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();
        bool firstTime = true;

        while (true)
        {
            NumberValue condition = (NumberValue)res.register(this.visit(node.condition_node, context));

            if (res.should_return())
            {
                return res;
            }

            if (!firstTime)
            {
                if (!condition.is_true())
                {
                    break;
                }
            }
            else
            {
                firstTime = false;
            }

            object value = res.register(this.visit(node.body_node, context));

            if (res.should_return() && !res.loop_should_continue && !res.loop_should_break)
            {
                return res;
            }

            if (res.loop_should_continue)
            {
                continue;
            }

            if (res.loop_should_break)
            {
                break;
            }
        }

        return res.success(Values.NULL);
    }

    public RuntimeResult visit_FuncDefNode(FuncDefNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        string func_name = node.var_name_tok == null ? "" : (string)node.var_name_tok.value;
        object body_node = node.body_node;

        List<Tuple<string, object>> arg_names = new List<Tuple<string, object>>();

        foreach (Tuple<Token, object> arg in node.arg_name_toks)
        {
            if (arg.Item2 == null)
            {
                arg_names.Add(new Tuple<string, object>(arg.Item1.value.ToString(), null));
            }
            else
            {
                arg_names.Add(new Tuple<string, object>(arg.Item1.value.ToString(), res.register(this.visit(arg.Item2, context))));

                if (res.error != null)
                {
                    return res;
                }
            }
        }

        FunctionValue func_value = new FunctionValue(func_name, body_node, arg_names, node.should_auto_return);
        func_value.set_context(context).set_pos(node.pos_start, node.pos_end);

        if (node.var_name_tok != null && node.var_name_tok.value.ToString() != "")
        {
            if (context.symbol_table.present(func_name))
            {
                if (context.symbol_table.get(func_name).GetType() == typeof(FunctionValue))
                {
                    int funcIdx = 1;

                    while (true)
                    {
                        if (context.symbol_table.present(func_name + "$" + funcIdx))
                        {
                            funcIdx++;
                            continue;
                        }

                        context.symbol_table.set(func_name + "$" + funcIdx, func_value);
                        break;
                    }
                }
                else
                {
                    context.symbol_table.set(func_name, func_value);
                }
            }
            else
            {
                context.symbol_table.set(func_name, func_value);
            }
        }

        return res.success(func_value);
    }

    public RuntimeResult visit_CallNode(CallNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();
        List<object> args = new List<object>();
        object value_to_call = res.register(this.visit(node.node_to_call, context));

        if (res.should_return())
        {
            return res;
        }

        if (value_to_call.GetType() == typeof(FunctionValue))
        {
            FunctionValue theFunc = (FunctionValue) value_to_call;
            int necessary = 0;

            foreach (Tuple<string, object> arg in theFunc.arg_names)
            {
                if (arg.Item2 != null)
                {
                    break;
                }

                necessary++;
            }

            if (node.arg_nodes.Count > theFunc.arg_names.Count)
            {
                int funcIdx = 1;

                while (true)
                {
                    if (context.symbol_table.present(theFunc.name + "$" + funcIdx))
                    {
                        FunctionValue newFunc = (FunctionValue) context.symbol_table.get(theFunc.name + "$" + funcIdx);
                        
                        if (node.arg_nodes.Count > newFunc.arg_names.Count)
                        {
                            funcIdx++;
                            continue;
                        }

                        value_to_call = newFunc;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (node.arg_nodes.Count < necessary)
            {
                int funcIdx = 1;

                while (true)
                {
                    if (context.symbol_table.present(theFunc.name + "$" + funcIdx))
                    {
                        FunctionValue newFunc = (FunctionValue)context.symbol_table.get(theFunc.name + "$" + funcIdx);
                        int newNecessary = 0;

                        foreach (Tuple<string, object> arg in newFunc.arg_names)
                        {
                            if (arg.Item2 != null)
                            {
                                break;
                            }

                            newNecessary++;
                        }

                        if (node.arg_nodes.Count < newNecessary)
                        {
                            funcIdx++;
                            continue;
                        }

                        value_to_call = newFunc;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            value_to_call = ((FunctionValue)value_to_call).copy().set_pos(node.pos_start, node.pos_end).set_context(context);

            foreach (object arg_node in node.arg_nodes)
            {
                args.Add(res.register(this.visit(arg_node, context)));

                if (res.should_return())
                {
                    return res;
                }
            }

            object return_value = res.register(((FunctionValue)value_to_call).execute(args));

            if (res.should_return())
            {
                return res;
            }

            return_value = return_value.GetType().GetMethod("copy").Invoke(return_value, new object[] { });
            return_value = return_value.GetType().GetMethod("set_pos").Invoke(return_value, new object[] { node.pos_start, node.pos_end });
            return_value = return_value.GetType().GetMethod("set_context").Invoke(return_value, new object[] { context });
            return res.success(return_value);
        }
        else
        {
            value_to_call = ((BuiltInFunction)value_to_call).copy().set_pos(node.pos_start, node.pos_end).set_context(context);

            foreach (object arg_node in node.arg_nodes)
            {
                args.Add(res.register(this.visit(arg_node, context)));

                if (res.should_return())
                {
                    return res;
                }
            }

            object return_value = res.register(((BuiltInFunction)value_to_call).execute(args));

            if (res.should_return())
            {
                return res;
            }

            return_value = return_value.GetType().GetMethod("copy").Invoke(return_value, new object[] { });
            return_value = return_value.GetType().GetMethod("set_pos").Invoke(return_value, new object[] { node.pos_start, node.pos_end });
            return_value = return_value.GetType().GetMethod("set_context").Invoke(return_value, new object[] { context });

            return res.success(return_value);
        }

        return res.success(Values.NULL);
    }

    public RuntimeResult visit_StringNode(StringNode node, Context context)
    {
        return new RuntimeResult().success(new StringValue(node.tok.value.ToString()).set_context(context).set_pos(node.pos_start, node.pos_end));
    }

    public RuntimeResult visit_ListNode(ListNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();
        List<object> elements = new List<object>();

        foreach (object element_node in node.element_nodes)
        {
            elements.Add(res.register(this.visit(element_node, context)));

            if (res.should_return())
            {
                return res;
            }
        }

        return res.success(new ListValue(elements).set_context(context).set_pos(node.pos_start, node.pos_end));
    }

    public RuntimeResult visit_ReturnNode(ReturnNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();
        object value = Values.NULL;

        if (node.node_to_return != null)
        {
            value = res.register(this.visit(node.node_to_return, context));

            if (res.should_return())
            {
                return res;
            }
        }

        return res.success_return(value);
    }

    public RuntimeResult visit_ContinueNode(ContinueNode node, Context context)
    {
        return new RuntimeResult().success_continue();
    }

    public RuntimeResult visit_BreakNode(BreakNode node, Context context)
    {
        return new RuntimeResult().success_break();
    }

    public RuntimeResult visit_ForEachNode(ForEachNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        object toVisit = res.register(this.visit(node.list_node, context));

        if (res.should_return())
        {
            return res;
        }

        if (toVisit.GetType() == typeof(ListValue))
        {
            ListValue theList = (ListValue)toVisit;

            foreach (object element in theList.elements)
            {
                if (res.loop_should_continue)
                {
                    continue;
                }

                if (res.loop_should_break)
                {
                    break;
                }

                context.symbol_table.set(node.element_var_name.value, element);
                res.register(this.visit(node.body_node, context));

                if (res.should_return())
                {
                    return res;
                }

                if (res.loop_should_continue)
                {
                    continue;
                }

                if (res.loop_should_break)
                {
                    break;
                }
            }
        }
        else if (toVisit.GetType() == typeof(SetValue))
        {
            SetValue theSet = (SetValue)toVisit;

            foreach (object element in theSet.elements)
            {
                if (res.loop_should_continue)
                {
                    continue;
                }

                if (res.loop_should_break)
                {
                    break;
                }

                context.symbol_table.set(node.element_var_name.value, element);
                res.register(this.visit(node.body_node, context));

                if (res.should_return())
                {
                    return res;
                }

                if (res.loop_should_continue)
                {
                    continue;
                }

                if (res.loop_should_break)
                {
                    break;
                }
            }
        }
        else if (toVisit.GetType() == typeof(StringValue))
        {
            StringValue theString = (StringValue)toVisit;
            List<object> elements = new List<object>();

            foreach (char c in theString.value.ToCharArray())
            {
                elements.Add(new StringValue(c.ToString()));
            }

            foreach (object element in elements)
            {
                if (res.loop_should_continue)
                {
                    continue;
                }

                if (res.loop_should_break)
                {
                    break;
                }

                context.symbol_table.set(node.element_var_name.value, element);
                res.register(this.visit(node.body_node, context));

                if (res.should_return())
                {
                    return res;
                }

                if (res.loop_should_continue)
                {
                    continue;
                }

                if (res.loop_should_break)
                {
                    break;
                }
            }
        }
        else
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "The specified value is not a list, a set or a string", context));
        }

        return res.success(Values.NULL);
    }

    public RuntimeResult visit_SwitchNode(SwitchNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();
        bool broken = false;

        foreach (Tuple<object, object> element in node.cases)
        {
            if (res.loop_should_break)
            {
                broken = true;

                break;
            }

            object expr = res.register(this.visit(element.Item1, context));

            if (res.should_return())
            {
                return res;
            }

            object variable = res.register(this.visit(node.comparisonExpr, context));

            if (res.should_return())
            {
                return res;
            }

            if ((string) (variable.GetType().GetMethod("as_string").Invoke(variable, new object[] { })) == (string) (expr.GetType().GetMethod("as_string").Invoke(expr, new object[] { })))
            {
                res.register(this.visit(element.Item2, context));

                if (res.should_return())
                {
                    return res;
                }
            }

            if (res.loop_should_break)
            {
                broken = true;

                break;
            }
        }

        if (!broken && node.default_case != null)
        {
            res.register(this.visit(node.default_case, context));

            if (res.should_return())
            {
                return res;
            }
        }

        return res.success(Values.NULL);
    }

    public RuntimeResult visit_StructDefNode(StructDefNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();
        StructValue value = new StructValue((string)node.var_name_tok.value, node.statements_node);

        value.set_context(context).set_pos(node.pos_start, node.pos_end);
        context.symbol_table.set(node.var_name_tok.value, value);

        return res.success(value);
    }

    public RuntimeResult visit_StructAccessNode(StructAccessNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        if (!context.symbol_table.present(node.var_name_tok.value))
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Could not find this struct", context));
        }

        try
        {
            StructValue theStruct = (StructValue)context.symbol_table.get(node.var_name_tok.value);

            if (!theStruct.context.symbol_table.present(node.access_var_name_tok.value))
            {
                return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Could not find this variable", context));
            }

            object value = theStruct.context.symbol_table.get(node.access_var_name_tok.value);

            return res.success(value);
        }
        catch 
        {
            BuiltInStruct theStruct = (BuiltInStruct)context.symbol_table.get(node.var_name_tok.value);

            if (!theStruct.context.symbol_table.present(node.access_var_name_tok.value))
            {
                return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Could not find this variable", context));
            }

            object value = theStruct.context.symbol_table.get(node.access_var_name_tok.value);

            return res.success(value);
        }

        return res.success(Values.NULL);
    }

    public RuntimeResult visit_StructReAssignNode(StructReAssignNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        if (!context.symbol_table.present(node.var_name_tok.value))
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Could not find this struct", context));
        }

        try
        {
            StructValue theStruct = (StructValue)context.symbol_table.get(node.var_name_tok.value);
            object value = null;

            if (node.node != null)
            {
                value = res.register(this.visit(node.node, context));

                if (res.should_return())
                {
                    return res;
                }
            }

            if (theStruct.context.symbol_table.present(node.access_var_name_tok.value))
            {
                if (!theStruct.context.symbol_table.can_be_rewrite(node.access_var_name_tok.value))
                {
                    return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Can not access to constants variables", context));
                }

                object actualValue = theStruct.context.symbol_table.get(node.access_var_name_tok.value);

                if (node.op_tok.type == "EQ")
                {
                    theStruct.context.symbol_table.set(node.access_var_name_tok.value, value);
                }
                else
                {
                    Tuple<object, Error> result = (Tuple<object, Error>)actualValue.GetType().GetMethod(node.op_tok.type.ToLower().Replace("_eq", "ed_by")).Invoke(actualValue, new object[] { value });
                    value = result.Item1;
                    theStruct.context.symbol_table.set(node.access_var_name_tok.value, value);
                }

                value = value.GetType().GetMethod("copy").Invoke(value, new object[] { });
                value.GetType().GetMethod("set_pos").Invoke(value, new object[] { node.pos_start, node.pos_end });

                return res.success(value);
            }
        }
        catch 
        {
            BuiltInStruct theStruct = (BuiltInStruct)context.symbol_table.get(node.var_name_tok.value);
            object value = null;

            if (node.node != null)
            {
                value = res.register(this.visit(node.node, context));

                if (res.should_return())
                {
                    return res;
                }
            }

            if (theStruct.context.symbol_table.present(node.access_var_name_tok.value))
            {
                if (!theStruct.context.symbol_table.can_be_rewrite(node.access_var_name_tok.value))
                {
                    return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Can not access to constants variables", context));
                }

                object actualValue = theStruct.context.symbol_table.get(node.access_var_name_tok.value);

                if (node.op_tok.type == "EQ")
                {
                    theStruct.context.symbol_table.set(node.access_var_name_tok.value, value);
                }
                else
                {
                    Tuple<object, Error> result = (Tuple<object, Error>)actualValue.GetType().GetMethod(node.op_tok.type.ToLower().Replace("_eq", "ed_by")).Invoke(actualValue, new object[] { value });
                    value = result.Item1;
                    theStruct.context.symbol_table.set(node.access_var_name_tok.value, value);
                }

                value = value.GetType().GetMethod("copy").Invoke(value, new object[] { });
                value.GetType().GetMethod("set_pos").Invoke(value, new object[] { node.pos_start, node.pos_end });

                return res.success(value);
            }
        }

        return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Variable is not created", context));
    }

    public RuntimeResult visit_StructCallNode(StructCallNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        List<object> arg_nodes = node.arg_nodes;
        List<object> args = new List<object>();

        if (!context.symbol_table.present(node.struct_var_name_tok.value))
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Could not find this struct", context));
        }

        try
        {
            StructValue theStruct = (StructValue)context.symbol_table.get(node.struct_var_name_tok.value);

            if (!theStruct.context.symbol_table.present(node.access_var_name_tok.value))
            {
                return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Could not find this variable", context));
            }

            object value_to_call = theStruct.context.symbol_table.get(node.access_var_name_tok.value);

            if (value_to_call.GetType() == typeof(FunctionValue))
            {
                FunctionValue theFunc = (FunctionValue)value_to_call;
                int necessary = 0;

                foreach (Tuple<string, object> arg in theFunc.arg_names)
                {
                    if (arg.Item2 != null)
                    {
                        break;
                    }

                    necessary++;
                }

                if (node.arg_nodes.Count > theFunc.arg_names.Count)
                {
                    int funcIdx = 1;

                    while (true)
                    {
                        if (theStruct.context.symbol_table.present(theFunc.name + "$" + funcIdx))
                        {
                            FunctionValue newFunc = (FunctionValue)theStruct.context.symbol_table.get(theFunc.name + "$" + funcIdx);

                            if (node.arg_nodes.Count > newFunc.arg_names.Count)
                            {
                                funcIdx++;
                                continue;
                            }

                            value_to_call = newFunc;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (node.arg_nodes.Count < necessary)
                {
                    int funcIdx = 1;

                    while (true)
                    {
                        if (theStruct.context.symbol_table.present(theFunc.name + "$" + funcIdx))
                        {
                            FunctionValue newFunc = (FunctionValue)theStruct.context.symbol_table.get(theFunc.name + "$" + funcIdx);
                            int newNecessary = 0;

                            foreach (Tuple<string, object> arg in newFunc.arg_names)
                            {
                                if (arg.Item2 != null)
                                {
                                    break;
                                }

                                newNecessary++;
                            }

                            if (node.arg_nodes.Count < newNecessary)
                            {
                                funcIdx++;
                                continue;
                            }

                            value_to_call = newFunc;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                value_to_call = ((FunctionValue)value_to_call).copy().set_pos(node.pos_start, node.pos_end).set_context(theStruct.context);

                foreach (object arg_node in node.arg_nodes)
                {
                    args.Add(res.register(this.visit(arg_node, context)));

                    if (res.should_return())
                    {
                        return res;
                    }
                }

                object return_value = res.register(((FunctionValue)value_to_call).execute(args));

                if (res.should_return())
                {
                    return res;
                }

                return_value = return_value.GetType().GetMethod("copy").Invoke(return_value, new object[] { });
                return_value = return_value.GetType().GetMethod("set_pos").Invoke(return_value, new object[] { node.pos_start, node.pos_end });
                return_value = return_value.GetType().GetMethod("set_context").Invoke(return_value, new object[] { theStruct.context });

                return res.success(return_value);
            }
            else
            {
                value_to_call = ((BuiltInFunction)value_to_call).copy().set_pos(node.pos_start, node.pos_end).set_context(theStruct.context);

                foreach (object arg_node in node.arg_nodes)
                {
                    args.Add(res.register(this.visit(arg_node, context)));

                    if (res.should_return())
                    {
                        return res;
                    }
                }

                object return_value = res.register(((BuiltInFunction)value_to_call).execute(args));

                if (res.should_return())
                {
                    return res;
                }

                return_value = return_value.GetType().GetMethod("copy").Invoke(return_value, new object[] { });
                return_value = return_value.GetType().GetMethod("set_pos").Invoke(return_value, new object[] { node.pos_start, node.pos_end });
                return_value = return_value.GetType().GetMethod("set_context").Invoke(return_value, new object[] { theStruct.context });

                return res.success(return_value);
            }
        }
        catch 
        {
            BuiltInStruct theStruct = (BuiltInStruct)context.symbol_table.get(node.struct_var_name_tok.value);

            if (!theStruct.context.symbol_table.present(node.access_var_name_tok.value))
            {
                return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Could not find this variable", context));
            }

            object value_to_call = theStruct.context.symbol_table.get(node.access_var_name_tok.value);

            if (value_to_call.GetType() == typeof(bool))
            {
                foreach (object arg_node in node.arg_nodes)
                {
                    args.Add(res.register(this.visit(arg_node, context)));

                    if (res.should_return())
                    {
                        return res;
                    }
                }

                object return_value = res.register(theStruct.exec_func(node.access_var_name_tok.value.ToString(), args));

                if (res.should_return())
                {
                    return res;
                }

                return res.success(return_value);
            }
        }

        return res.success(Values.NULL);
    }

    public RuntimeResult visit_LabelNode(LabelNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        context.symbol_table.set(node.var_name_tok.value, new LabelValue(node.var_name_tok.value.ToString(), node.statements), false);

        res.register(this.visit(node.statements, context));

        if (res.should_return())
        {
            return res;
        }

        return res.success(Values.NULL);
    }
    
    public RuntimeResult visit_GotoNode(GotoNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        if (!context.symbol_table.present(node.var_name_tok.value))
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Label not found", context));
        }

        object value = context.symbol_table.get(node.var_name_tok.value);

        if (value.GetType() == typeof(LabelValue))
        {
            res.register(this.visit((((LabelValue)value)).statements, context));

            if (res.should_return())
            {
                return res;
            }
        }
        else
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "This value is not a label", context));
        }

        return res.success(Values.NULL);
    }

    public RuntimeResult visit_NamespaceDefNode(NamespaceDefNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();
        NamespaceValue value = new NamespaceValue(node.var_name_tok.value.ToString(), node.statements_node, this, node.pos_start, node.pos_end, context);

        context.symbol_table.set(node.var_name_tok.value, value);

        return res.success(value);
    }

    public RuntimeResult visit_NamespaceAccessNode(NamespaceAccessNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        if (!context.symbol_table.present(node.var_name_tok.value))
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Could not find this namespace", context));
        }

        try
        {
            NamespaceValue theNamespace = (NamespaceValue)context.symbol_table.get(node.var_name_tok.value);

            if (!theNamespace.context.symbol_table.present(node.access_var_name_tok.value))
            {
                return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Could not find this variable", context));
            }

            object value = theNamespace.context.symbol_table.get(node.access_var_name_tok.value);

            return res.success(value);
        }
        catch 
        {
            BuiltInNamespace theNamespace = (BuiltInNamespace)context.symbol_table.get(node.var_name_tok.value);

            if (!theNamespace.context.symbol_table.present(node.access_var_name_tok.value))
            {
                return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Could not find this variable", context));
            }

            object value = theNamespace.context.symbol_table.get(node.access_var_name_tok.value);

            return res.success(value);
        }

        return res.success(Values.NULL);
    }

    public RuntimeResult visit_NamespaceReAssignNode(NamespaceReAssignNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        if (!context.symbol_table.present(node.var_name_tok.value))
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Could not find this namespace", context));
        }

        try
        {
            NamespaceValue theNamespace = (NamespaceValue)context.symbol_table.get(node.var_name_tok.value);
            object value = null;

            if (node.node != null)
            {
                value = res.register(this.visit(node.node, context));

                if (res.should_return())
                {
                    return res;
                }
            }

            if (theNamespace.context.symbol_table.present(node.access_var_name_tok.value))
            {
                if (!theNamespace.context.symbol_table.can_be_rewrite(node.access_var_name_tok.value))
                {
                    return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Can not access to constants variables", context));
                }

                object actualValue = theNamespace.context.symbol_table.get(node.access_var_name_tok.value);

                if (node.op_tok.type == "EQ")
                {
                    theNamespace.context.symbol_table.set(node.access_var_name_tok.value, value);
                }
                else
                {
                    Tuple<object, Error> result = (Tuple<object, Error>)actualValue.GetType().GetMethod(node.op_tok.type.ToLower().Replace("_eq", "ed_by")).Invoke(actualValue, new object[] { value });
                    value = result.Item1;
                    theNamespace.context.symbol_table.set(node.access_var_name_tok.value, value);
                }

                value = value.GetType().GetMethod("copy").Invoke(value, new object[] { });
                value.GetType().GetMethod("set_pos").Invoke(value, new object[] { node.pos_start, node.pos_end });

                return res.success(value);
            }
        }
        catch 
        {
            BuiltInNamespace theNamespace = (BuiltInNamespace)context.symbol_table.get(node.var_name_tok.value);
            object value = null;

            if (node.node != null)
            {
                value = res.register(this.visit(node.node, context));

                if (res.should_return())
                {
                    return res;
                }
            }

            if (theNamespace.context.symbol_table.present(node.access_var_name_tok.value))
            {
                if (!theNamespace.context.symbol_table.can_be_rewrite(node.access_var_name_tok.value))
                {
                    return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Can not access to constants variables", context));
                }

                object actualValue = theNamespace.context.symbol_table.get(node.access_var_name_tok.value);

                if (node.op_tok.type == "EQ")
                {
                    theNamespace.context.symbol_table.set(node.access_var_name_tok.value, value);
                }
                else
                {
                    Tuple<object, Error> result = (Tuple<object, Error>)actualValue.GetType().GetMethod(node.op_tok.type.ToLower().Replace("_eq", "ed_by")).Invoke(actualValue, new object[] { value });
                    value = result.Item1;
                    theNamespace.context.symbol_table.set(node.access_var_name_tok.value, value);
                }

                value = value.GetType().GetMethod("copy").Invoke(value, new object[] { });
                value.GetType().GetMethod("set_pos").Invoke(value, new object[] { node.pos_start, node.pos_end });

                return res.success(value);
            }
        }

        return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Variable is not created", context));
    }

    public RuntimeResult visit_NamespaceCallNode(NamespaceCallNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        List<object> arg_nodes = node.arg_nodes, args = new List<object>();

        if (!context.symbol_table.present(node.struct_var_name_tok.value))
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Could not find this namespace", context));
        }

        try
        {
            NamespaceValue theNamespace = (NamespaceValue)context.symbol_table.get(node.struct_var_name_tok.value);

            if (!theNamespace.context.symbol_table.present(node.access_var_name_tok.value))
            {
                return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Could not find this variable", context));
            }

            object value_to_call = theNamespace.context.symbol_table.get(node.access_var_name_tok.value);

            if (value_to_call.GetType() == typeof(FunctionValue))
            {
                FunctionValue theFunc = (FunctionValue)value_to_call;
                int necessary = 0;

                foreach (Tuple<string, object> arg in theFunc.arg_names)
                {
                    if (arg.Item2 != null)
                    {
                        break;
                    }

                    necessary++;
                }

                if (node.arg_nodes.Count > theFunc.arg_names.Count)
                {
                    int funcIdx = 1;

                    while (true)
                    {
                        if (theNamespace.context.symbol_table.present(theFunc.name + "$" + funcIdx))
                        {
                            FunctionValue newFunc = (FunctionValue)theNamespace.context.symbol_table.get(theFunc.name + "$" + funcIdx);

                            if (node.arg_nodes.Count > newFunc.arg_names.Count)
                            {
                                funcIdx++;
                                continue;
                            }

                            value_to_call = newFunc;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (node.arg_nodes.Count < necessary)
                {
                    int funcIdx = 1;

                    while (true)
                    {
                        if (theNamespace.context.symbol_table.present(theFunc.name + "$" + funcIdx))
                        {
                            FunctionValue newFunc = (FunctionValue)theNamespace.context.symbol_table.get(theFunc.name + "$" + funcIdx);
                            int newNecessary = 0;

                            foreach (Tuple<string, object> arg in newFunc.arg_names)
                            {
                                if (arg.Item2 != null)
                                {
                                    break;
                                }

                                newNecessary++;
                            }

                            if (node.arg_nodes.Count < newNecessary)
                            {
                                funcIdx++;
                                continue;
                            }

                            value_to_call = newFunc;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                value_to_call = ((FunctionValue)value_to_call).copy().set_pos(node.pos_start, node.pos_end).set_context(theNamespace.context);

                foreach (object arg_node in node.arg_nodes)
                {
                    args.Add(res.register(this.visit(arg_node, context)));

                    if (res.should_return())
                    {
                        return res;
                    }
                }

                object return_value = res.register(((FunctionValue)value_to_call).execute(args));

                if (res.should_return())
                {
                    return res;
                }

                return_value = return_value.GetType().GetMethod("copy").Invoke(return_value, new object[] { });
                return_value = return_value.GetType().GetMethod("set_pos").Invoke(return_value, new object[] { node.pos_start, node.pos_end });
                return_value = return_value.GetType().GetMethod("set_context").Invoke(return_value, new object[] { theNamespace.context });

                return res.success(return_value);
            }
            else
            {
                value_to_call = ((BuiltInFunction)value_to_call).copy().set_pos(node.pos_start, node.pos_end).set_context(theNamespace.context);

                foreach (object arg_node in node.arg_nodes)
                {
                    args.Add(res.register(this.visit(arg_node, context)));

                    if (res.should_return())
                    {
                        return res;
                    }
                }

                object return_value = res.register(((BuiltInFunction)value_to_call).execute(args));

                if (res.should_return())
                {
                    return res;
                }

                return_value = return_value.GetType().GetMethod("copy").Invoke(return_value, new object[] { });
                return_value = return_value.GetType().GetMethod("set_pos").Invoke(return_value, new object[] { node.pos_start, node.pos_end });
                return_value = return_value.GetType().GetMethod("set_context").Invoke(return_value, new object[] { theNamespace.context });

                return res.success(return_value);
            }
        }
        catch 
        {
            BuiltInNamespace theNamespace = (BuiltInNamespace)context.symbol_table.get(node.struct_var_name_tok.value);

            if (!theNamespace.context.symbol_table.present(node.access_var_name_tok.value))
            {
                return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Could not find this variable", context));
            }

            object value_to_call = theNamespace.context.symbol_table.get(node.access_var_name_tok.value);

            if (value_to_call.GetType() == typeof(bool))
            {
                foreach (object arg_node in node.arg_nodes)
                {
                    args.Add(res.register(this.visit(arg_node, context)));

                    if (res.should_return())
                    {
                        return res;
                    }
                }

                object return_value = res.register(theNamespace.exec_func(node.access_var_name_tok.value.ToString(), args));

                if (res.should_return())
                {
                    return res;
                }

                return res.success(return_value);
            }
        }

        return res.success(Values.NULL);
    }

    public RuntimeResult visit_VarListAccessNode(VarListAccessNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        if (!context.symbol_table.present(node.var_name_tok.value))
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "This list/set/string is not present in the memory", context));
        }

        if (context.symbol_table.get(node.var_name_tok.value).GetType() != typeof(ListValue) && context.symbol_table.get(node.var_name_tok.value).GetType() != typeof(SetValue) && context.symbol_table.get(node.var_name_tok.value).GetType() != typeof(StringValue))
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "The specified variable is not a list/set/string", context));
        }

        object indexValue = res.register(this.visit(node.access_node, context));

        if (res.should_return())
        {
            return res;
        }

        if (indexValue.GetType() != typeof(NumberValue))
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Index value needs to be a number", context));
        }

        if ((int) ((NumberValue) indexValue).value < 0)
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Index value needs to be greater or equal than zero", context));
        }

        if (context.symbol_table.get(node.var_name_tok.value).GetType() == typeof(SetValue))
        {
            if ((int)((NumberValue)indexValue).value >= ((SetValue)context.symbol_table.get(node.var_name_tok.value)).elements.Count)
            {
                return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Index value can not be greater than the number of the elements of the set", context));
            }

            return res.success(((SetValue)context.symbol_table.get(node.var_name_tok.value)).elements[(int)((NumberValue)indexValue).value]);
        }
        else if (context.symbol_table.get(node.var_name_tok.value).GetType() == typeof(StringValue))
        {
            if ((int)((NumberValue)indexValue).value >= ((StringValue)context.symbol_table.get(node.var_name_tok.value)).value.Length)
            {
                return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Index value can not be greater than the number of the elements of the string", context));
            }

            return res.success(new StringValue(((StringValue)context.symbol_table.get(node.var_name_tok.value)).value[(int)((NumberValue)indexValue).value].ToString()));
        }

        if ((int)((NumberValue)indexValue).value >= ((ListValue)context.symbol_table.get(node.var_name_tok.value)).elements.Count)
        {
            return res.failure(new RuntimeError(node.pos_start, node.pos_end, "Index value can not be greater than the number of the elements of the list", context));
        }

        return res.success(((ListValue)context.symbol_table.get(node.var_name_tok.value)).elements[(int)((NumberValue)indexValue).value]);
    }

    public RuntimeResult visit_SetNode(SetNode node, Context context)
    {
        RuntimeResult res = new RuntimeResult();

        List<object> elements = new List<object>();

        foreach (object element_node in node.element_nodes)
        {
            elements.Add(res.register(this.visit(element_node, context)));

            if (res.should_return())
            {
                return res;
            }
        }

        return res.success(new SetValue(elements).set_context(context).set_pos(node.pos_start, node.pos_end).fix());
    }
}