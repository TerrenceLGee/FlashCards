﻿using System.Diagnostics.CodeAnalysis;

namespace Flashcards.Core.Models;

public class Stack
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    protected Stack(){}

    [SetsRequiredMembers]
    public Stack(string name)
    {
        Name = name;
    }

    [SetsRequiredMembers]
    public Stack(int id, string name)
        : this(name)
    {
        Id = id;
    }
}