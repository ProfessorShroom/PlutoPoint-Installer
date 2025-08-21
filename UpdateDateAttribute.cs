using System;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class AssemblyUpdateDateAttribute : Attribute
{
    public string Date { get; }
    public AssemblyUpdateDateAttribute(string date) => Date = date;
}