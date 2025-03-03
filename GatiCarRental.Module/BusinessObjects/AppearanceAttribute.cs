using System;

namespace GatiCarRental.Module.BusinessObjects
{
    internal class AppearanceAttribute : Attribute
    {
        private string Context;
        private string Criteria;
        private bool Enabled;
        private string v;

        public AppearanceAttribute(string v, bool Enabled, string Criteria, string Context)
        {
            this.v = v;
            this.Enabled = Enabled;
            this.Criteria = Criteria;
            this.Context = Context;
        }
    }
}