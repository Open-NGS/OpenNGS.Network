using UnityEngine;
using System.Collections;

namespace OpenNGS.Platform.Pay
{
    /// <summary>
	/// Response object for payment callback
	/// </summary>
	[System.Serializable]
    public class NPayResponse : JsonSerializable
    {
		///<summary>
		///payment result,0 is success,other is fail。
		/// 
		///</summary>
		[JsonProp("resultCode")]
		public int resultCode { get; set; }

		///<summary>
		///inner error code for payment。
		///</summary>
		[JsonProp("resultInerCode")]
		public string resultInerCode { get; set; }

		///<summary>
		///game coin quantity。
		///</summary>
		[JsonProp("realSaveNum")]
		public string realSaveNum { get; set; }

		///<summary>
		///pay channel。
		///</summary>
		[JsonProp("payChannel")]
		public string payChannel { get; set; }

		///<summary>
		///error message for payment
		///</summary>
		[JsonProp("resultMsg")]
		public string resultMsg { get; set; }

		///<summary>
		///extend info, this will return the original value that passed in Request
		///</summary>
		[JsonProp("appExtends")]
		public string appExtends { get; set; }


		///<summary>
		///business type, game or goods or month or subscribe
		///</summary>
		[JsonProp("reqType")]
		public string reqType { get; set; }

		///<summary>
		/// Reserved field
		///</summary>
		[JsonProp("payReserve1")]
		public string payReserve1 { get; set; }

		///<summary>
		/// Reserved field
		///</summary>
		[JsonProp("payReserve2")]
		public string payReserve2 { get; set; }

		///<summary>
		/// Reserved field
		///</summary>
		[JsonProp("payReserve3")]
		public string payReserve3 { get; set; }

		public NPayResponse(string param) : base(param) { }

		public NPayResponse(object json) : base(json) { }
	}
}
