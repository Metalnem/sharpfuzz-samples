// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: libfuzzer-http.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Libfuzzer.Http {

  /// <summary>Holder for reflection information generated from libfuzzer-http.proto</summary>
  public static partial class LibfuzzerHttpReflection {

    #region Descriptor
    /// <summary>File descriptor for libfuzzer-http.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static LibfuzzerHttpReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChRsaWJmdXp6ZXItaHR0cC5wcm90bxIObGliZnV6emVyLmh0dHAiJQoGSGVh",
            "ZGVyEgwKBG5hbWUYASABKAkSDQoFdmFsdWUYAiABKAkihAEKB1JlcXVlc3QS",
            "JgoGbWV0aG9kGAEgASgOMhYubGliZnV6emVyLmh0dHAuTWV0aG9kEgwKBHBh",
            "dGgYAiABKAkSDAoEaG9zdBgDIAEoCRInCgdoZWFkZXJzGAQgAygLMhYubGli",
            "ZnV6emVyLmh0dHAuSGVhZGVyEgwKBGJvZHkYBSABKAwqagoGTWV0aG9kEgcK",
            "A0dFVBAAEggKBEhFQUQQARIICgRQT1NUEAISBwoDUFVUEAMSCgoGREVMRVRF",
            "EAQSCwoHQ09OTkVDVBAFEgsKB09QVElPTlMQBhIJCgVUUkFDRRAHEgkKBVBB",
            "VENIEAhiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Libfuzzer.Http.Method), }, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Libfuzzer.Http.Header), global::Libfuzzer.Http.Header.Parser, new[]{ "Name", "Value" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Libfuzzer.Http.Request), global::Libfuzzer.Http.Request.Parser, new[]{ "Method", "Path", "Host", "Headers", "Body" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum Method {
    [pbr::OriginalName("GET")] Get = 0,
    [pbr::OriginalName("HEAD")] Head = 1,
    [pbr::OriginalName("POST")] Post = 2,
    [pbr::OriginalName("PUT")] Put = 3,
    [pbr::OriginalName("DELETE")] Delete = 4,
    [pbr::OriginalName("CONNECT")] Connect = 5,
    [pbr::OriginalName("OPTIONS")] Options = 6,
    [pbr::OriginalName("TRACE")] Trace = 7,
    [pbr::OriginalName("PATCH")] Patch = 8,
  }

  #endregion

  #region Messages
  public sealed partial class Header : pb::IMessage<Header> {
    private static readonly pb::MessageParser<Header> _parser = new pb::MessageParser<Header>(() => new Header());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Header> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Libfuzzer.Http.LibfuzzerHttpReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Header() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Header(Header other) : this() {
      name_ = other.name_;
      value_ = other.value_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Header Clone() {
      return new Header(this);
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 1;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "value" field.</summary>
    public const int ValueFieldNumber = 2;
    private string value_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Value {
      get { return value_; }
      set {
        value_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Header);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Header other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Name != other.Name) return false;
      if (Value != other.Value) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (Value.Length != 0) hash ^= Value.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Name.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Name);
      }
      if (Value.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Value);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (Value.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Value);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Header other) {
      if (other == null) {
        return;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.Value.Length != 0) {
        Value = other.Value;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Name = input.ReadString();
            break;
          }
          case 18: {
            Value = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed partial class Request : pb::IMessage<Request> {
    private static readonly pb::MessageParser<Request> _parser = new pb::MessageParser<Request>(() => new Request());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Request> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Libfuzzer.Http.LibfuzzerHttpReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Request() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Request(Request other) : this() {
      method_ = other.method_;
      path_ = other.path_;
      host_ = other.host_;
      headers_ = other.headers_.Clone();
      body_ = other.body_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Request Clone() {
      return new Request(this);
    }

    /// <summary>Field number for the "method" field.</summary>
    public const int MethodFieldNumber = 1;
    private global::Libfuzzer.Http.Method method_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Libfuzzer.Http.Method Method {
      get { return method_; }
      set {
        method_ = value;
      }
    }

    /// <summary>Field number for the "path" field.</summary>
    public const int PathFieldNumber = 2;
    private string path_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Path {
      get { return path_; }
      set {
        path_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "host" field.</summary>
    public const int HostFieldNumber = 3;
    private string host_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Host {
      get { return host_; }
      set {
        host_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "headers" field.</summary>
    public const int HeadersFieldNumber = 4;
    private static readonly pb::FieldCodec<global::Libfuzzer.Http.Header> _repeated_headers_codec
        = pb::FieldCodec.ForMessage(34, global::Libfuzzer.Http.Header.Parser);
    private readonly pbc::RepeatedField<global::Libfuzzer.Http.Header> headers_ = new pbc::RepeatedField<global::Libfuzzer.Http.Header>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Libfuzzer.Http.Header> Headers {
      get { return headers_; }
    }

    /// <summary>Field number for the "body" field.</summary>
    public const int BodyFieldNumber = 5;
    private pb::ByteString body_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString Body {
      get { return body_; }
      set {
        body_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Request);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Request other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Method != other.Method) return false;
      if (Path != other.Path) return false;
      if (Host != other.Host) return false;
      if(!headers_.Equals(other.headers_)) return false;
      if (Body != other.Body) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Method != 0) hash ^= Method.GetHashCode();
      if (Path.Length != 0) hash ^= Path.GetHashCode();
      if (Host.Length != 0) hash ^= Host.GetHashCode();
      hash ^= headers_.GetHashCode();
      if (Body.Length != 0) hash ^= Body.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Method != 0) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Method);
      }
      if (Path.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Path);
      }
      if (Host.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(Host);
      }
      headers_.WriteTo(output, _repeated_headers_codec);
      if (Body.Length != 0) {
        output.WriteRawTag(42);
        output.WriteBytes(Body);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Method != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Method);
      }
      if (Path.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Path);
      }
      if (Host.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Host);
      }
      size += headers_.CalculateSize(_repeated_headers_codec);
      if (Body.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(Body);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Request other) {
      if (other == null) {
        return;
      }
      if (other.Method != 0) {
        Method = other.Method;
      }
      if (other.Path.Length != 0) {
        Path = other.Path;
      }
      if (other.Host.Length != 0) {
        Host = other.Host;
      }
      headers_.Add(other.headers_);
      if (other.Body.Length != 0) {
        Body = other.Body;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Method = (global::Libfuzzer.Http.Method) input.ReadEnum();
            break;
          }
          case 18: {
            Path = input.ReadString();
            break;
          }
          case 26: {
            Host = input.ReadString();
            break;
          }
          case 34: {
            headers_.AddEntriesFrom(input, _repeated_headers_codec);
            break;
          }
          case 42: {
            Body = input.ReadBytes();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
