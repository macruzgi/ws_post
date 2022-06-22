using Kinpos.comm;
using Kinpos.Dcl;
using Kinpos.Dcl.Core;

using Kinpos.Dcl.Comm;
using Kinpos.Dcl.Util;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

//libreria
using Newtonsoft.Json;

//libreria para mysql
//using MySql.Data.MySqlClient;
//libreria para sql server
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Descripción breve de ws_tas_post
/// </summary>
//[WebService(Namespace = "http://tempuri.org/")]
[WebService(Namespace = "http://192.168.100.117:50919/ws_tas_post.asmx")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
// [System.Web.Script.Services.ScriptService]
public class ws_tas_post : System.Web.Services.WebService
{

    public ws_tas_post()
    {

        //Elimine la marca de comentario de la línea siguiente si utiliza los componentes diseñados 
        //InitializeComponent(); 
    }
    public List<byte[]> SetParametersResponse()
    {
        List<byte[]> parameters = new List<byte[]>();

        parameters.Add(DCL_TAG._TxnRspCode);
        parameters.Add(DCL_TAG._TxnAuthNum);
        parameters.Add(DCL_TAG._TxnRRN);
        parameters.Add(DCL_TAG._TxnInvoice);
        parameters.Add(DCL_TAG._TxnSTAN);
        parameters.Add(DCL_TAG._TxnRspText);
        parameters.Add(DCL_TAG._TxnTID);
        parameters.Add(DCL_TAG._TxnMerName);
        parameters.Add(DCL_TAG._AppName);
        parameters.Add(DCL_TAG._idBankCode);
        parameters.Add(DCL_TAG._TxnAcqID);
        parameters.Add(DCL_TAG._TxnHolderName);
        parameters.Add(DCL_TAG._TxnMaskPAN);
        parameters.Add(DCL_TAG._TxnBaseAmt);
        parameters.Add(DCL_TAG._TxnExpedientNum);

        return parameters;

    }
    public ResponseTransaction GetResponse(DCL_Result returndata)
    {

        ResponseTransaction response = new ResponseTransaction()
        {
            ResponseCode = returndata.GetValue_ASCII2String(0),
            AuthorizationCode = returndata.GetValue_ASCII2String(1),
            RRNN = returndata.GetValue_ASCII2String(2),
            Invoice = returndata.GetValue_BCD2String(3),
            STAN = returndata.GetValue_ASCII2String(4),
            ResponseText = returndata.GetValue_ASCII2String(5),
            TerminalId = returndata.GetValue_ASCII2String(6),
            EmisorName = returndata.GetValue_ASCII2String(7),
            MerchantId = returndata.GetValue_ASCII2String(8)
        };

        return response;
    }
    public DCL_Ethernet CreateDCLEthernet()
    {

        DCL_Ethernet dcl = new DCL_Ethernet();
        // dcl.IP = "192.168.0.77";
        dcl.Port = 5000;
        dcl.Timeout = 50000;
        dcl.setCurrency("0840");

        dcl.setPrintBeforeSendData(false);
        //dcl.setVerifyLastDigits(); // solicita verificacion d elos ultimos 4 digitos de la tarjeta cuando ha sido lectura de banda

        dcl.ProcessBIN = true;
        dcl.OpenChannel = true;
        return dcl;

    }
    public string ValidarAcceso(string p_clave, string p_usuario)
    {
       
        SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion_SQLSERVER"].ConnectionString);
        //abro conexion
        string estado = "No hay conexion";
        conexion.Open();
        //if(conexion.State == ConnectionState.Open )
       // {
            //estado = "Conexión abierta";
           // conexion.Close();
       // }
        SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM adm_usuario where usuario_clave = HASHBYTES('MD5', @p_clave) and  usuario_nombre =@p_usuario", conexion);
        //agregando los parametros
        /*SqlParameter parametros = new SqlParameter();
        parametros.ParameterName = "@p_clave";
        parametros.Value = p_clave;
        parametros.ParameterName = "@p_usuario";
        parametros.Value = p_usuario;
        cmd.Parameters.Add(parametros);
        */
        cmd.Parameters.Add("@p_clave", SqlDbType.Char);
        cmd.Parameters["@p_clave"].Value = p_clave;
        cmd.Parameters.Add("@p_usuario", SqlDbType.Char);
        cmd.Parameters["@p_usuario"].Value = p_usuario;
        estado = cmd.ExecuteScalar().ToString();
        conexion.Close();
        return estado;

    }
    [WebMethod]
    public string HelloWorld()
    {
        return  "Se ha logrado conectar al WS";
    }
    [WebMethod]
    public string COMPRAR(string monto, string ip, string p_clave, string p_usuario)
    {
        
        string respuesta = "Algo Salió mal";
        respuesta = ValidarAcceso(p_clave, p_usuario);
        if (respuesta == "0")
        {
            var json = new
            {
                _ResponseCode = "00",
                _ResponseText = "Aceso DENEGADO",
            };
             
            return JsonConvert.SerializeObject(json);
        }


        try
        {
            DCL_Ethernet dcl = this.CreateDCLEthernet();
            //string monto = txtMonto.Text;
            monto = monto.Replace(".", string.Empty);

            // MessageBox.Show(monto, "respuesta");
            //  return;
            dcl.IP = ip;
            dcl.setAmount(monto);

            DCL_Result returndata = dcl.Sale(this.SetParametersResponse());

            ResponseTransaction response = this.GetResponse(returndata);
            //MessageBox.Show(response.AuthorizationCode, "respuesta");

            if (response.ResponseCode.Equals("00"))
            {
                //respuesta = response.Invoice;
                var json = new
                {
                    _ResponseCode = response.ResponseCode,
                    _AuthorizationCode = response.AuthorizationCode,
                    _RRNN = response.RRNN,
                    _Invoice = response.Invoice,
                    _STAN = response.STAN,
                    _ResponseText = response.ResponseText,
                    _TerminalId = response.TerminalId,
                    _EmisorName = response.EmisorName,
                    _MerchantId = response.MerchantId
                };
                respuesta =  JsonConvert.SerializeObject(json);
            }
            else
            {
               // respuesta = response.ResponseText;
                var json = new
            {
                _ResponseCode = response.ResponseCode,
                _AuthorizationCode = response.AuthorizationCode,
                _RRNN = response.RRNN,
                _Invoice = response.Invoice,
                _STAN = response.STAN,
                _ResponseText = response.ResponseText,
                _TerminalId = response.TerminalId,
                _EmisorName = response.EmisorName,
                _MerchantId = response.MerchantId
            };
                respuesta = JsonConvert.SerializeObject(json);
            }

            dcl.CloseChannel();
           
    
        }
        catch (Exception ex)
        {


        }
        return respuesta;
    }
    [WebMethod]
    public string ANULAR(string numero_factura, string ip)
    {
        string respuesta = "Algo salió mal";
        try
        {
            DCL_Ethernet dcl = this.CreateDCLEthernet();
            dcl.IP = ip;
            dcl.setInvoice(numero_factura);
            // MessageBox.Show(monto, "respuesta");
            //  return;
            DCL_Result returndata = dcl.Void(this.SetParametersResponse());

            ResponseTransaction response = this.GetResponse(returndata);
            if (response != null)
            {
                if (response.ResponseCode.Equals("00"))
                {
                    respuesta = response.Invoice;
                }
                else
                {
                    respuesta = response.ResponseText;
                }
                
            }
           else
            {
                respuesta = "Algo salió mal";
            }

            dcl.CloseChannel();

        }
        catch (Exception ex)
        {
            
        }
        return respuesta;
    }
}
