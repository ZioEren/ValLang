using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

public class StructHttpClient
{
    public bool already;
    public Dictionary<string, string> headers = new Dictionary<string, string>();

    public void execute_declare(Context exec_ctx)
    {
        if (!already)
        {
            exec_ctx.symbol_table.set("url", new StringValue(""));
            exec_ctx.symbol_table.set("setUrl", true);
            exec_ctx.symbol_table.set("getUrl", true);
            exec_ctx.symbol_table.set("method", new StringValue(""));
            exec_ctx.symbol_table.set("setMethod", true);
            exec_ctx.symbol_table.set("getMethod", true);
            exec_ctx.symbol_table.set("set", true);
            exec_ctx.symbol_table.set("setHeader", true);
            exec_ctx.symbol_table.set("getHeader", true);
            exec_ctx.symbol_table.set("delHeader", true);
            exec_ctx.symbol_table.set("send", true);
            exec_ctx.symbol_table.set("clearHeaders", true);
            already = true;
        }    
    }

    public RuntimeResult execute_getUrl(Context exec_ctx)
    {
        return new RuntimeResult().success(exec_ctx.symbol_table.get("url"));
    }

    public List<string> get_getUrl()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_setUrl(Context exec_ctx)
    {
        object newUrl = exec_ctx.symbol_table.get("newUrl");

        if (newUrl.GetType() == typeof(StringValue))
        {
            exec_ctx.symbol_table.set("url", newUrl);
            return new RuntimeResult().success(newUrl);
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Argument must be string", exec_ctx));
    }

    public List<string> get_setUrl()
    {
        List<string> arg_names = new List<string>();

        arg_names.Add("newUrl");

        return arg_names;
    }

    public RuntimeResult execute_setMethod(Context exec_ctx)
    {
        object newMethod = exec_ctx.symbol_table.get("newMethod");

        if (newMethod.GetType() == typeof(StringValue))
        {
            string theMethod = ((StringValue)newMethod).value.ToLower();

            if (theMethod == "get" || theMethod == "post" || theMethod == "delete" || theMethod == "put" || theMethod == "patch")
            {
                exec_ctx.symbol_table.set("method", new StringValue(theMethod));
                return new RuntimeResult().success(new StringValue(theMethod));
            }

            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "The method must be 'GET', 'POST', 'DELETE', 'PUT' or 'PATCH'", exec_ctx));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Argument must be string", exec_ctx));
    }

    public List<string> get_setMethod()
    {
        List<string> arg_names = new List<string>();

        arg_names.Add("newMethod");

        return arg_names;
    }

    public RuntimeResult execute_getMethod(Context exec_ctx)
    {
        return new RuntimeResult().success(exec_ctx.symbol_table.get("method"));
    }

    public List<string> get_getMethod()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_set(Context exec_ctx)
    {
        object newUrl = exec_ctx.symbol_table.get("newUrl");

        if (newUrl.GetType() == typeof(StringValue))
        {
            exec_ctx.symbol_table.set("url", newUrl);
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "The url must be string", exec_ctx));
        }

        object newMethod = exec_ctx.symbol_table.get("newMethod");

        if (newMethod.GetType() == typeof(StringValue))
        {
            string theMethod = ((StringValue)newMethod).value.ToLower();

            if (theMethod == "get" || theMethod == "post" || theMethod == "delete" || theMethod == "put" || theMethod == "patch")
            {
                exec_ctx.symbol_table.set("method", new StringValue(theMethod));
                return new RuntimeResult().success(Values.NULL);
            }

            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "The method must be 'GET', 'POST', 'DELETE', 'PUT' or 'PATCH'", exec_ctx));
        }

        return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "The method must be string", exec_ctx));
    }

    public List<string> get_set()
    {
        List<string> arg_names = new List<string>();

        arg_names.Add("newUrl");
        arg_names.Add("newMethod");

        return arg_names;
    }

    public RuntimeResult execute_setHeader(Context exec_ctx)
    {
        object header = exec_ctx.symbol_table.get("header");
        object value = exec_ctx.symbol_table.get("value");

        if (header.GetType() == typeof(StringValue))
        {
            if (value.GetType() == typeof(StringValue))
            {
                headers.Add(((StringValue) header).value, ((StringValue) value).value);
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "The header name must be string", exec_ctx));
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "The header name must be string", exec_ctx));
        }

        return new RuntimeResult().success(value);
    }

    public List<string> get_setHeader()
    {
        List<string> arg_names = new List<string>();

        arg_names.Add("header");
        arg_names.Add("value");

        return arg_names;
    }


    public RuntimeResult execute_getHeader(Context exec_ctx)
    {
        object header = exec_ctx.symbol_table.get("header");

        if (header.GetType() == typeof(StringValue))
        {
            if (headers.ContainsKey(((StringValue) header).value))
            {
                return new RuntimeResult().success(new StringValue(headers[((StringValue)header).value]));
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Can not find that header", exec_ctx));
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "The header name must be string", exec_ctx));
        }

        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_getHeader()
    {
        List<string> arg_names = new List<string>();

        arg_names.Add("header");

        return arg_names;
    }

    public RuntimeResult execute_send(Context exec_ctx)
    {
        var http = new HttpClient();

        try
        {
            var request = new HttpRequestMessage();

            request.RequestUri = new Uri(((StringValue)exec_ctx.symbol_table.get("url")).value);
            request.Method = new HttpMethod(((StringValue) exec_ctx.symbol_table.get("method")).value.ToUpper());

            try
            {
                foreach (string theHeader in headers.Keys)
                {
                    request.Headers.Add(theHeader, headers[theHeader]);
                }
            }
            catch (Exception)
            {
            }

            return new RuntimeResult().success(http.SendAsync(request).Result.Content.ReadAsStringAsync().Result.ToString());
        }
        catch (Exception)
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Failed to send the request", exec_ctx));
        }

        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_send()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_clearHeaders(Context exec_ctx)
    {
        headers.Clear();
        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_clearHeaders()
    {
        List<string> arg_names = new List<string>();
        return arg_names;
    }

    public RuntimeResult execute_delHeader(Context exec_ctx)
    {
        object header = exec_ctx.symbol_table.get("header");

        if (header.GetType() == typeof(StringValue))
        {
            if (headers.ContainsKey(((StringValue)header).value))
            {
                object oldValue = new StringValue(headers[((StringValue)header).value]);
                headers.Remove(((StringValue)header).value);
                return new RuntimeResult().success(oldValue);
            }
            else
            {
                return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "Can not find that header", exec_ctx));
            }
        }
        else
        {
            return new RuntimeResult().failure(new RuntimeError(new Position(0, 0, 0, "", ""), new Position(0, 0, 0, "", ""), "The header name must be string", exec_ctx));
        }

        return new RuntimeResult().success(Values.NULL);
    }

    public List<string> get_delHeader()
    {
        List<string> arg_names = new List<string>();

        arg_names.Add("header");

        return arg_names;
    }
}