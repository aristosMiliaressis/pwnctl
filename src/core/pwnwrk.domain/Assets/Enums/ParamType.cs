namespace pwnwrk.domain.Assets.Enums;

public enum ParamType
{
    Query,
    Body,
    Path, // if segment is integer or guid or md5 or email
    Cookie,
    Header,
}