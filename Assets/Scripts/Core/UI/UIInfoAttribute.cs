using System;

namespace ProjectBase.UI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class UIInfoAttribute : Attribute
    {
        public int ID;

        public UIInfoAttribute(int id)
        {
            ID = id;
        }
    }

}