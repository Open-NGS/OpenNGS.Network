using System.Collections.Generic;
using OpenNGS.Exchange.Data;
using OpenNGS.Exchange.Common;
using Systems;
using OpenNGS.Item.Data;
using static UnityEngine.GraphicsBuffer;

namespace OpenNGS.Systems
{ 
    public class NgExchangeSystem : GameSubSystem<NgExchangeSystem>, INgExchangeSystem
    {
        private IItemSystem m_itemSys = null;

        protected override void OnCreate()
        {
            m_itemSys = App.GetService<IItemSystem>();
            base.OnCreate();
        }

        public override string GetSystemName()
        {
            return "NgExchange";
        }

        public ExchangeRsp ExchangeItem(ExchangeReq request)
        {
            ExchangeResultType _resultType = ExchangeResultType.Success;
            ExchangeRsp response = new ExchangeRsp();
            
            //TODO检查来源物体是否满足条件
            foreach(SourceState src in request.src)
            {
                if(src.BUseGrid == true)
                {

                }
                else
                {

                }
            }

            //TODO检查目标物体是否可以添加

            foreach(TargetState trg in request.target)
            {

            }

            response.result = (uint)_resultType;
            return response;
        }
    }
}

