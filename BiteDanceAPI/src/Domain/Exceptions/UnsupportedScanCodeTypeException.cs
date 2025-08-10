namespace BiteDanceAPI.Domain.Exceptions;

public class UnsupportedScanCodeTypeException(CodeType code)
    : Exception($"Scan code type \"{code}\" is unsupported.");
