using ProtoBuf.Meta;
namespace OpenNGS.Core
{
	public partial class NGSText : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			Key = null;
		}
		public void OnRelease()
		{
			Key = null;
		}
		public void OnSpawn()
		{
		}
		public static NGSText SpawnFromPool()
		{
			return (NGSText)OpenNGS.Network.ProtoPool.Instance.Get(typeof(NGSText));
		}
	}
	public partial class NGSAttributes : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
		}
		public void OnRelease()
		{
		}
		public void OnSpawn()
		{
		}
		public static NGSAttributes SpawnFromPool()
		{
			return (NGSAttributes)OpenNGS.Network.ProtoPool.Instance.Get(typeof(NGSAttributes));
		}
	}
	public partial class NGSVoid : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
		}
		public void OnRelease()
		{
		}
		public void OnSpawn()
		{
		}
		public static NGSVoid SpawnFromPool()
		{
			return (NGSVoid)OpenNGS.Network.ProtoPool.Instance.Get(typeof(NGSVoid));
		}
	}
	public partial class NGSVector2 : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			X = 0;
			Y = 0;
		}
		public void OnRelease()
		{
			X = 0;
			Y = 0;
		}
		public void OnSpawn()
		{
		}
		public static NGSVector2 SpawnFromPool()
		{
			return (NGSVector2)OpenNGS.Network.ProtoPool.Instance.Get(typeof(NGSVector2));
		}
	}
	public partial class NGSVector3 : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			X = 0;
			Y = 0;
			Z = 0;
		}
		public void OnRelease()
		{
			X = 0;
			Y = 0;
			Z = 0;
		}
		public void OnSpawn()
		{
		}
		public static NGSVector3 SpawnFromPool()
		{
			return (NGSVector3)OpenNGS.Network.ProtoPool.Instance.Get(typeof(NGSVector3));
		}
	}
	public partial class NGSVector4 : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			X = 0;
			Y = 0;
			Z = 0;
			W = 0;
		}
		public void OnRelease()
		{
			X = 0;
			Y = 0;
			Z = 0;
			W = 0;
		}
		public void OnSpawn()
		{
		}
		public static NGSVector4 SpawnFromPool()
		{
			return (NGSVector4)OpenNGS.Network.ProtoPool.Instance.Get(typeof(NGSVector4));
		}
	}
	public partial class NGSMessage : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			topic_name = null;
			producer_id = 0;
			message_id = 0;
			payload = null;
		}
		public void OnRelease()
		{
			topic_name = null;
			producer_id = 0;
			message_id = 0;
			payload = null;
		}
		public void OnSpawn()
		{
		}
		public static NGSMessage SpawnFromPool()
		{
			return (NGSMessage)OpenNGS.Network.ProtoPool.Instance.Get(typeof(NGSMessage));
		}
	}
	public partial class NGSResult : OpenNGS.IProtoExtension
	{
		public void Clear()
		{
			Result = 0;
			Message = null;
		}
		public void OnRelease()
		{
			Result = 0;
			Message = null;
		}
		public void OnSpawn()
		{
		}
		public static NGSResult SpawnFromPool()
		{
			return (NGSResult)OpenNGS.Network.ProtoPool.Instance.Get(typeof(NGSResult));
		}
	}
	public class NGSPBFactory
	{
		public static void  AutoRegistePBFactory()
		{
			 RuntimeTypeModel.Default.Add(typeof(NGSText), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(NGSAttributes), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(NGSVoid), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(NGSVector2), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(NGSVector3), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(NGSVector4), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(NGSMessage), true).SetFactory("SpawnFromPool");
			 RuntimeTypeModel.Default.Add(typeof(NGSResult), true).SetFactory("SpawnFromPool");
		}
	}
}
