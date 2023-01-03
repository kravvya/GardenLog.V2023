﻿using System.Linq.Expressions;
using System.Reflection;

namespace GardenLog.SharedKernel
{
    // This can be modified to BaseEntity<TId> to support multiple key types (e.g. Guid)
    public abstract class BaseEntity
    {
        public string Id { get; set; }

        public List<BaseDomainEvent> DomainEvents = new();


        public void Set<T>(Expression<Func<T>> prop, T value)
        {
            var expr = (MemberExpression)prop.Body;
            var mem = (PropertyInfo)expr.Member;

            var currentValue = mem.GetValue(this);

            if (currentValue == null && value == null)
                return;

            if ((currentValue == null && value != null)
                || (currentValue != null && value == null)
                || !currentValue.Equals(value))
            {
                mem.SetValue(this, value);
                AddDomainEvent(mem.Name);
            }
        }

        protected abstract void AddDomainEvent(string attributeName);
    }
}