using System;

namespace DemoApi.Api.Tests.Common.Configuration;

[AttributeUsage(AttributeTargets.Method)]
public class TestPriorityAttribute(int priority) : Attribute
{
    public int Priority { get; } = priority;
}