So, it seems like the issue shows up here in `DataPortalProxy.CreateRequest()`. It's really not a question per-se about `FlowSecurityPrincipalFromClient` (I don't even see where this is coming into play), but how the serializer should handle a `null`. Looking [here](https://github.com/MarimerLLC/csla/blob/0f27cda24471d7a80286412a83b829023b77a922/Source/Csla/Serialization/Mobile/MobileFormatter.cs#L102), it uses something called a [`NullPlaceholder`](https://github.com/MarimerLLC/csla/blob/main/Source/Csla/Serialization/Mobile/NullPlaceholder.cs), and "serializes" an instance of that.

I'm thinking that if we're given a `null` object, we store the string `"NULL"` and that's it. On the deserialization side, if the string is `"NULL"`, we return `null`.

OK, fixed the `null` issue in the formatter, but now getting a new exception:

```
  Message: 
System.InvalidCastException : Unable to cast object of type 'Csla.Server.Hosts.DataPortalChannel.UpdateRequest' to type 'Csla.Server.Hosts.DataPortalChannel.DataPortalResponse'.

  Stack Trace: 
DataPortalProxy.Update(ICslaObject obj, DataPortalContext context, Boolean isSync)
DataPortalTests.RoundtripAsync() line 37
GenericAdapter`1.BlockUntilCompleted()
NoMessagePumpStrategy.WaitForCompletion(AwaitAdapter awaiter)
AsyncToSyncAdapter.Await[TResult](TestExecutionContext context, Func`1 invoke)
AsyncToSyncAdapter.Await(TestExecutionContext context, Func`1 invoke)
TestMethodCommand.RunTestMethod(TestExecutionContext context)
TestMethodCommand.Execute(TestExecutionContext context)
<>c__DisplayClass3_0.<PerformWork>b__0()
<>c__DisplayClass1_0`1.<DoIsolated>b__0(Object _)
ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
ContextUtils.DoIsolated(ContextCallback callback, Object state)
ContextUtils.DoIsolated[T](Func`1 func)
SimpleWorkItem.PerformWork()
1)    at Csla.DataPortalClient.DataPortalProxy.Update(ICslaObject obj, DataPortalContext context, Boolean isSync)
DataPortalTests.RoundtripAsync() line 37
GenericAdapter`1.BlockUntilCompleted()
NoMessagePumpStrategy.WaitForCompletion(AwaitAdapter awaiter)
AsyncToSyncAdapter.Await[TResult](TestExecutionContext context, Func`1 invoke)
AsyncToSyncAdapter.Await(TestExecutionContext context, Func`1 invoke)
TestMethodCommand.RunTestMethod(TestExecutionContext context)
TestMethodCommand.Execute(TestExecutionContext context)
<>c__DisplayClass3_0.<PerformWork>b__0()
<>c__DisplayClass1_0`1.<DoIsolated>b__0(Object _)
ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
ContextUtils.DoIsolated(ContextCallback callback, Object state)
ContextUtils.DoIsolated[T](Func`1 func)
SimpleWorkItem.PerformWork()
```