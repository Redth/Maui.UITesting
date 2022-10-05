# App Agent Spec

A library built into and running inside of the Application being tested is the foundation for retrieving view hierarchy information and performing some sets of actions within the running application.

## Terms / Definitions
- App Agent - The library running within the application being tested/automated
- Driver - The library for orchestrating the automation of the app being tested, typically referenced by a unit test

## App Agent Client vs Driver Server

Due to security and permission implications in mobile app environments, it is much easier for an App to connect out as a client to a server than to host its own server listening locally on the device.  For this reason the client/server relationship may seems inverted from the intuitive choice.

The App Agent acts as the _client_ and the Driver acts as the _server_.

The flow of a session is:
1. Driver starts a gRPC server listening for connections.
2. Driver sets up the app session by starting and configuring emulators or devices and installing and launching the app as appropriate for the given configuration.
3. Driver enters a waiting state blocking all automation API calls until an App Agent connection has been established.
4. App calls entry point into the Agent library when it starts.
5. Agent library initiates a connection to the Driver's gRPC server.
6. Driver continues with any waiting and future automation API calls.

## gRPC RemoteApp Service

```proto
service RemoteApp {
	rpc GetElementsRoute(stream ElementsResponse) returns (stream ElementsRequest) {}
	rpc PerformActionRoute(stream PerformActionResponse) returns (stream PerformActionRequest) {}
}
```

## GetElementsRoute

This is a duplex stream gRPC route served by the _Driver_ which is responsible for receiving `ElementRequest`s from the _App Agent_ and issuing `ElementResponse`s for those requests.

```proto
message ElementsRequest {
	string requestId = 1;
	Platform platform = 2;
}
```

Each request is required to have a unique `requestId` value.  The server stores this identifier and later uses it to resume the blocking Driver task when it receives a response for the same identifier.  The request also specifies the automation platform that it expects a view tree representation of back for.  In a .NET MAUI app this could be either `Maui` or the native platform the app is running in (ie: `Android` or `iOS`, etc.).  This provides the Driver with some flexibility for automating various parts of the app.  For example, if a native dialog is displayed, it is helpful to query the native view tree to find the native 'Ok' button to click.

The response to this request contains both the corresponding `requestId` as mentioned, and also a collection of `Element` objects which represent the view tree:

```proto
message ElementsResponse {
	string requestId = 1;
	repeated Element elements = 2;
}
```

The `Element` object has a number of fields representing the view it represents:

```proto
message Element {
	
	// Unique ID to represent the view
	string id = 1;
	
	// The ID of the parent of this element if it is parented
	optional string parentId = 2;
	
	// The Automation ID (often accessibility id)
	optional string automationId = 3;
	
	// Automation Platform (ie: Maui, or Android)
	Platform platform = 4;
	
	// Class type name of the UI element
	string type = 5;
	
	// Full class type name
	string fullType = 6;
	
	// Text of the UI element (ie: the visible string contents of a label or textbox)
	optional string text = 7;
	
	// Visibility of the element
	bool visible = 8;
	
	// Element enabled state
	bool enabled = 9;
	
	// Focus state
	bool focused = 10;
	
	// The local position and size (often relative to the parent)
	Frame viewFrame = 11;
	
	// The position and size relative to the containing window
	Frame windowFrame = 12;
	
	// The position and size relative to the screen
	Frame screenFrame = 13;
	
	// Density of the size for the given display
	double density = 15;
	
	// Children of the element
	repeated Element children = 14;
}
```


> In this inverted relationship, the _Driver_ actually writes `ElementRequest` objects to the _Response stream_ and the _App Agent_ responds with `ElementResponse` objects over the _Request stream_.

### ElementRequest
 -

## PerformActionRoute

