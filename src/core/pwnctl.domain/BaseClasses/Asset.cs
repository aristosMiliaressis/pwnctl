﻿using pwnctl.kernel.BaseClasses;
using System.Text;
using System.Security.Cryptography;

namespace pwnctl.domain.BaseClasses
{
    public abstract class Asset : Entity<string>, IEquatable<Asset>
    {
        public abstract override string ToString();

        public override bool Equals(object obj)
        {
            return obj is Asset asset && ToString().Equals(asset.ToString());
        }

        public bool Equals(Asset asset)
        {
            return Equals((object)asset);
        }

        public static bool operator ==(Asset left, Asset right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Asset left, Asset right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
