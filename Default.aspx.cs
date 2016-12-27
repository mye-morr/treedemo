using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Goldtect.ASTreeViewDemo
{
    public partial class Default : System.Web.UI.Page
    {
         protected void LoginUser_LoggedIn(object sender, EventArgs e)
         {
             MembershipUser mbsUser= Membership.GetUser(LoginUser.UserName);
             Session["UserName"] = mbsUser.UserName;
             Response.Redirect("DnDSaveDB.aspx");
         }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

    }
}