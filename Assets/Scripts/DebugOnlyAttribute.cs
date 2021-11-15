using Sirenix.OdinInspector;
using System;

namespace UAM
{
    [IncludeMyAttributes()]
    [ShowIf("@useDebug == true")]
    [VerticalGroup("Debug")]
    public class DebugOnlyAttribute : Attribute
    {

    }


}
