﻿using System.Threading.Tasks;

namespace Imparter.Handling
{
    public interface IHandle<in T> where T : class
    {
        Task Handle(T message);
    }
}
