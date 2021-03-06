﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ENode.Infrastructure;

namespace ENode.Eventing
{
    public class DefaultEventPersistenceSynchronizerProvider : IEventPersistenceSynchronizerProvider, IAssemblyInitializer
    {
        private IList<IEventPersistenceSynchronizer> _synchronizerList = new List<IEventPersistenceSynchronizer>();

        public void Initialize(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var synchronizerType in assembly.GetExportedTypes().Where(x => IsEventPersistenceSynchronizer(x)))
                {
                    RegisterSynchronizer(synchronizerType);
                }
            }
        }

        public IEnumerable<IEventPersistenceSynchronizer> GetSynchronizers(EventStream eventStream)
        {
            return _synchronizerList.Where(x => x.IsSynchronizeTo(eventStream)).ToList();
        }

        private void RegisterSynchronizer(Type synchronizerType)
        {
            if (!_synchronizerList.Any(x => x.GetType() == synchronizerType))
            {
                _synchronizerList.Add(ObjectContainer.Resolve(synchronizerType) as IEventPersistenceSynchronizer);
            }
        }
        private bool IsEventPersistenceSynchronizer(Type type)
        {
            return type != null && type.IsClass && !type.IsAbstract && typeof(IEventPersistenceSynchronizer).IsAssignableFrom(type);
        }
    }
}
