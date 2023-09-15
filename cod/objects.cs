using System;

namespace cod
{

    public enum ObjectType
    {
        BOOLEAN,
        INTEGER,
        NULL,
        FLOAT,
        STRING
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
        public bool Value { get; }

        public Boolean(bool value)
        {
            Value = value;
        }

        public override ObjectType Type => ObjectType.BOOLEAN;

        public override string Inspect()
        {
            return Value.ToString().ToLower();
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
}