using System;

namespace cod
{

    public enum ObjectType
    {
        BOOLEAN,
        INTEGER,
        NULL,
        FLOAT,
        STRING,
        RETURN,
        ERROR,
        FUNCTION,

    }

    public abstract class Object
    {
        public abstract ObjectType Type { get; }
        public abstract string Inspect();
    }

    public class Inte : Object
    {
        public int Value { get; }

        public Inte(int value)
        {
            Value = value;
        }

        public override ObjectType Type => ObjectType.INTEGER;

        public override string Inspect()
        {
            return Value.ToString();
        }
    }

    public class Boolean : Object
    {
        public bool Value { get; set; }

        public Boolean(bool value)
        {
            Value = value;
        }

        public override ObjectType Type
        {
            get { return ObjectType.BOOLEAN; }
        }

        public override string Inspect()
        {
            return Value ? "verdadero" : "falso";
        }
    }
    public class Return : Object
    {
        public Object Value { get; }

        public Return(Object value)
        {
            Value = value;
        }

        public override ObjectType Type => ObjectType.RETURN;

        public override string Inspect()
        {
            return Value.Inspect();
        }

    }



    public class Null : Object
    {
        public override ObjectType Type => ObjectType.NULL;

        public override string Inspect()
        {
            return "nulo";
        }
    }

    public class Error : Object
    {
        public string Message { get; }

        public Error(string message)
        {
            Message = message;
        }

        public override ObjectType Type => ObjectType.ERROR;

        public override string Inspect()
        {
            return "ERROR: " + Message;
        }
    }



    public class Environment : Dictionary<string, Object>
    {
        private Environment _outer;
        private Dictionary<string, Object> _store;

        public Environment(Environment outer = null)
        {
            _store = new Dictionary<string, Object>();
            _outer = outer;
        }

        public Object Get(string key)
        {
            if (_store.ContainsKey(key))
            {
                return _store[key];
            }

            if (_outer != null)
            {
                return _outer.Get(key);
            }

            return null;
        }
        public void Set(string key, Object value)
        {
            _store[key] = value;
        }

        public void Remove(string key)
        {
            _store.Remove(key);
        }
    }

    public class Fnction : Object
    {
        public List<Identifier> Parameters { get; }
        public Block Body { get; }
        public Environment Env { get; }

        public Fnction(List<Identifier> parameters, Block body, Environment env)
        {
            Parameters = parameters;
            Body = body;
            Env = env;
        }

        public override ObjectType Type => ObjectType.FUNCTION;

        public override string Inspect()
        {
            var paramList = new List<string>();
            foreach (var parameter in Parameters)
            {
                paramList.Add(parameter.ToString());
            }

            var parameters = string.Join(", ", paramList);

            return $"procedimiento({parameters}) {Body}";
        }

    }

    public class String : Object
    {
        public string Value { get; }

        public String(string value)
        {
            Value = value;
        }

        public override ObjectType Type => ObjectType.STRING;

        public override string Inspect()
        {
            return Value;
        }
    }
}