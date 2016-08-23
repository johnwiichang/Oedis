using System;

namespace Oedis
{
    /// <summary>
    /// This attribute will let Oedis set propery as master record.
    /// </summary>
    public class MasterAttribute : Attribute { }

    /// <summary>
    /// This attribute will let Oedis set propery as reference record.
    /// Attention: Current version of Odeis can only set one reference property. If you set multiple reference properties, Oedis will just map the first one in CLR.
    /// </summary>
    public class ReferenceAttribute : Attribute { }

    /// <summary>
    /// This attribute will let Oedis ignore property.
    /// </summary>
    public class ExceptAttribute : Attribute { }
}
