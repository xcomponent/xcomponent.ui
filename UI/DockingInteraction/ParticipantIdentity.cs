using System;
using System.Collections.Generic;

namespace XComponent.Common.UI.DockingInteraction
{
    public class ParticipantIdentity
    {
        public event EventHandler<IdentityChangedEventArgs> Changed;

        private string _uniqueName;
        private IParticipantType _participantType;

        public ParticipantIdentity(IParticipantType participantType, string uniqueName)
        {
            _participantType = participantType;
            _uniqueName = uniqueName;
        }

        public IParticipantType ParticipantType
        {
            get { return _participantType; }
            private set { _participantType = value; }
        }

        public string UniqueName
        {
            get { return _uniqueName; }
            set
            {
                if (_uniqueName != value)
                {
                    string oldName = _uniqueName;
                    _uniqueName = value;
                    
                    OnChanged(new IdentityChangedEventArgs(oldName, value));
                }
            }
        }

        protected virtual void OnChanged(IdentityChangedEventArgs e)
        {
            EventHandler<IdentityChangedEventArgs> handler = Changed;
            if (handler != null) handler(this, e);
        }

        private sealed class UniqueNameParticipantTypeEqualityComparer : IEqualityComparer<ParticipantIdentity>
        {
            public bool Equals(ParticipantIdentity x, ParticipantIdentity y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x._uniqueName, y._uniqueName) && Equals(x._participantType, y._participantType);
            }

            public int GetHashCode(ParticipantIdentity obj)
            {
                unchecked
                {
                    return ((obj._uniqueName != null ? obj._uniqueName.GetHashCode() : 0)*397) ^ (obj._participantType != null ? obj._participantType.GetHashCode() : 0);
                }
            }
        }

        private static readonly IEqualityComparer<ParticipantIdentity> UniqueNameParticipantTypeComparerInstance = new UniqueNameParticipantTypeEqualityComparer();

        public static IEqualityComparer<ParticipantIdentity> UniqueNameParticipantTypeComparer
        {
            get { return UniqueNameParticipantTypeComparerInstance; }
        }
    }
}