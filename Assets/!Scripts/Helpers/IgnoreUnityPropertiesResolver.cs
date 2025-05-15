using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

public class IgnoreUnityPropertiesResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        // ���������� ������������ �������� Unity
        if (member.DeclaringType == typeof(UnityEngine.Component) ||
            member.DeclaringType == typeof(UnityEngine.GameObject) ||
            member.DeclaringType == typeof(UnityEngine.Object))
        {
            property.Ignored = true;
        }

        // ���������� ���������� ���������� ��������
        if (member.Name == "rigidbody" || member.Name == "collider" || member.Name == "animation")
        {
            property.Ignored = true;
        }

        return property;
    }
}