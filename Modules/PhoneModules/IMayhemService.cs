using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace PhoneModules
{
    [ServiceContract(Name = "IMayhemService")]
    public interface IMayhemService
    {
        [OperationContract]
        [WebGet(UriTemplate = "Event/{text}", BodyStyle = WebMessageBodyStyle.Bare)]
        void Event(string text);

        [OperationContract]
        [WebInvoke]
        void SetHtml(string html);

        [OperationContract]
        [WebInvoke]
        void SetInsideDiv(string insideDiv);

        [OperationContract]
        [WebGet]
        Stream Html(bool update);

        [OperationContract]
        [WebGet(UriTemplate = "Images/{id}")]
        Stream Images(string id);

        [OperationContract]
        [WebGet]
        void ShuttingDown();
    }
}
