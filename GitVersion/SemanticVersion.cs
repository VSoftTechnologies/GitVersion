namespace GitVersion
{
    using System;

    public class SemanticVersion
    {
        public string Suffix;
        public int Major;
        public int Minor;
        public int Patch;
        public SemanticVersionTag Tag;
        public int? PreReleasePartTwo;

        public SemanticVersion()
        {
            Tag = new SemanticVersionTag();
        }

        public bool Equals(SemanticVersion obj)
        {
            if (obj == null)
            {
                return false;
            }
            return Major == obj.Major &&
                   Minor == obj.Minor &&
                   Patch == obj.Patch &&
                   Tag == obj.Tag &&
                   PreReleasePartTwo == obj.PreReleasePartTwo &&
                   Suffix == obj.Suffix;
        }

        public static bool operator ==(SemanticVersion v1, SemanticVersion v2)
        {
            if (ReferenceEquals(v1, null))
            {
                return ReferenceEquals(v2, null);
            }
            return v1.Equals(v2);
        }
        
        public static bool operator !=(SemanticVersion v1, SemanticVersion v2)
        {
            return !(v1 == v2);
        }
        
        public static bool operator >(SemanticVersion v1, SemanticVersion v2)
        {
            return (v2 < v1);
        }

        public static bool operator >=(SemanticVersion v1, SemanticVersion v2)
        {
            return (v2 <= v1);
        }

        public static bool operator <=(SemanticVersion v1, SemanticVersion v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }
            return (v1.CompareTo(v2) <= 0);
        }

        public static bool operator <(SemanticVersion v1, SemanticVersion v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }
            return (v1.CompareTo(v2) < 0);
        }

        public int CompareTo(SemanticVersion value)
        {
            if (value == null)
            {
                return 1;
            }
            if (Major != value.Major)
            {
                if (Major > value.Major)
                {
                    return 1;
                }
                return -1;
            }
            if (Minor != value.Minor)
            {
                if (Minor > value.Minor)
                {
                    return 1;
                }
                return -1;
            }
            if (Patch != value.Patch)
            {
                if (Patch > value.Patch)
                {
                    return 1;
                }
                return -1;
            }
            if (Tag != value.Tag)
            {
                if (Tag > value.Tag)
                {
                    return 1;
                }
                return -1;
            }
            if (PreReleasePartTwo != value.PreReleasePartTwo)
            {
                if (PreReleasePartTwo > value.PreReleasePartTwo)
                {
                    return 1;
                }
                return -1;
            }
            return -1;
        }
    }
}