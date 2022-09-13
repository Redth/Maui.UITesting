using System;
namespace Microsoft.Maui.Automation.RemoteGrpc;

public interface IResponseMessage : IRequestIdentity, Google.Protobuf.IMessage
{
}

public interface IRequestMessage : IRequestIdentity, Google.Protobuf.IMessage
{
}

public interface IRequestIdentity
{
    string RequestId { get; set; }
}

public partial class ElementsRequest : IRequestMessage
{
}

public partial class ElementsResponse : IResponseMessage
{
}

public partial class PropertyRequest : IRequestMessage
{
}

public partial class PropertyResponse : IResponseMessage
{
}

public partial class FindElementsRequest : IRequestMessage
{
}


public partial class PerformActionRequest : IRequestMessage
{
}

public partial class PerformActionResponse : IResponseMessage
{
}

