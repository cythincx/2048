using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Elong.WebMS.ViewModel;
using Elong.WebMS.Model.Entity;
using Elong.WebMS.DA;
using Elong.WebMS.Service;
using Elong.WebMs.Service;
using Elong.Framework.MVC;
using Elong.Framework.DataAccess;
using System.Data;
using System.Globalization;
using Elong.Common.Utility;
using Elong.WebMS.Model.Enum;
using Elong.Common.Channel;
using Elong.Common.Log;

namespace Elong.WebMS.Channel.Controller
{
    /// <summary>
    /// orderFromManage界面和orderFromDetermine界面
    /// </summary>
    public class OrderFromManageController : WebMSBaseController
    {

        private OrderFromDetermineFacade orderFromDetermineFacade = ServiceFactory.OrderFromDetermineService;
        private OrderFromManageFacade orderFromManageFacade = ServiceFactory.OrderFromManageService;
        private List<OrderFrom> orderFromManageList = new List<OrderFrom>();
        private List<OrderFrom_Determine> orderFrom_DetermineList = new List<OrderFrom_Determine>();

        /// <summary>
        /// OrderFrom管理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderFromManage()
        {
            OrderFromManageViewModel orderFromManageVM = new OrderFromManageViewModel();
            orderFromManageVM.ParentDic = new Dictionary<int, string>();
            orderFromManageVM.ParentDic = orderFromManageFacade.GetParentIdAndNameDic();
            orderFromManageVM.IsHasFuncTypeName = IsHasFuncTypeName(CurrentMSUser.MSRoleIds, EFuncTypeName.OrderFrom_Operation);
            return View(orderFromManageVM);
        }

        /// <summary>
        /// OrderFromDetermine界面
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderFromDetermine(string orderFromId)
        {
            OrderFromDetermineViewModel orderFromDetermineVM = new OrderFromDetermineViewModel();
            int orderFromIdInt = Convert.ToInt32(orderFromId);

            OrderFrom_Determine orderFrom_Determine = new OrderFrom_Determine();
            orderFrom_Determine.CategoryId = orderFromIdInt;

            List<OrderFrom_Determine> orderFrom_DetermineList = this.GetOrderFrom_DetermineByCategoryId(orderFrom_Determine).ToList();

            if (orderFrom_DetermineList == null||orderFrom_DetermineList.Count == 0) 
            {
                orderFromDetermineVM.CategoryId = orderFromIdInt;
            }
            else
            {
                OrderFrom_Determine orderFrom_DetermineTemp = orderFrom_DetermineList[0];
                orderFromDetermineVM.PkId = Convert.ToInt32(orderFrom_DetermineTemp.Pkid);
                orderFromDetermineVM.CategoryId = Convert.ToInt32(orderFrom_DetermineTemp.CategoryId);
                orderFromDetermineVM.SHostName = orderFrom_DetermineTemp.SHostName;
                orderFromDetermineVM.SParamRegex = orderFrom_DetermineTemp.SParamRegex;
                orderFromDetermineVM.THostName = orderFrom_DetermineTemp.THostName;
                orderFromDetermineVM.TParamRegex = orderFrom_DetermineTemp.TParamRegex;
                orderFromDetermineVM.Priority = Convert.ToInt32(orderFrom_DetermineTemp.Priority);
                if (orderFrom_DetermineTemp.Op_date != null)
                {
                    orderFromDetermineVM.Op_date = Convert.ToDateTime(orderFrom_DetermineTemp.Op_date);
                }
                orderFromDetermineVM.Operator = orderFrom_DetermineTemp.Operator;
            }

            return View(orderFromDetermineVM);
        }

        #region ajax method

        /// <summary>
        /// 添加OrderFrom记录
        /// </summary>
        /// <param name="pkId"></param>
        /// <param name="name"></param>
        /// <param name="parentId"></param>
        /// <param name="coefficient"></param>
        /// <param name="intro"></param>
        /// <param name="status"></param>
        /// <param name="priority"></param>
        /// <param name="makeComeFrom"></param>
        /// <param name="proxyId"></param>
        /// <param name="saveCookies"></param>
        /// <param name="cookiesDays"></param>
        /// <param name="op_date"></param>
        /// <param name="operater"></param>
        /// <param name="isUseFParam"></param>
        /// <param name="fParamFormat"></param> 
        /// <param name="orderFromType"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        [Ajax]
        public OperationStatus AddOrderFrom(string pkId, string name, string parentId, string coefficient, string intro, string status, string priority,
                    string makeComeFrom, string proxyId, string saveCookies, string cookiesDays, string op_date, string operater, string isUseFParam,
                    string fParamFormat, string orderFromType, string notes)
        {

            if (String.IsNullOrEmpty(operater))
            {
                //try
                //{
                    operater = ServiceFactory.MSUser.GetMSUser(CurrentMSUser.CardNo).UserName;
                //}
                //catch (Exception e)
                //{
                  //  WebLog.ChannelLog.CommonLogger.Error("获取当前登录用户名异常：" + e.Message, e);
                //}
            }
            
            OrderFrom orderFrom = BuildOrderFrom(pkId, name, parentId, coefficient, intro, status, priority,
                    makeComeFrom, proxyId, saveCookies, cookiesDays, DateTime.Now.ToString(), operater, isUseFParam,
                    fParamFormat, orderFromType, notes);

            try
            {
                orderFromManageFacade.AddOrderFrom(orderFrom);
                return OperationStatus.Success;
            }
            catch (Exception e)
            {
                WebLog.ChannelLog.CommonLogger.Error("插入OrderFrom记录异常：" + e.Message, e);
                return OperationStatus.Error;
            }

        }

        /// <summary>
        /// 添加OrderFrom_Determine记录
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="categoryId"></param>
        /// <param name="sHostName"></param>
        /// <param name="sParamRegex"></param>
        /// <param name="tHostName"></param>
        /// <param name="tParamRegex"></param>
        /// <param name="priority"></param>
        /// <param name="op_date"></param>
        /// <param name="operater"></param>
        /// <returns></returns>
        [Ajax]
        public OperationStatus AddOrderFrom_Determine(string pkid, string categoryId, string sHostName,
                            string sParamRegex, string tHostName, string tParamRegex, string priority, string op_date, string operater)
        {

            if (String.IsNullOrEmpty(operater))
            {

                operater = ServiceFactory.MSUser.GetMSUser(CurrentMSUser.CardNo).UserName;
                //if (operater == null)
                //{
                
                //}
            }
            OrderFrom_Determine orderFrom_Determine = BuildOrderFrom_Determine("", categoryId, sHostName,
                            sParamRegex, tHostName, tParamRegex, priority, DateTime.Now.ToString(), operater);

            try
            {
                bool addResult = orderFromDetermineFacade.AddOrderFrom_Determine(orderFrom_Determine);
                if (addResult)
                {
                    return OperationStatus.Success;
                }
                else {
                    return OperationStatus.CategoryError;
                }
            }
            catch (Exception e)
            {
                WebLog.ChannelLog.CommonLogger.Error("插入OrderFrom_Determine记录异常：" + e.Message, e);
                return OperationStatus.Error;
            }
        }

        /// <summary>
        /// 更新OrderFrom
        /// </summary>
        /// <param name="pkId"></param>
        /// <param name="name"></param>
        /// <param name="parentId"></param>
        /// <param name="coefficient"></param>
        /// <param name="intro"></param>
        /// <param name="status"></param>
        /// <param name="priority"></param>
        /// <param name="makeComeFrom"></param>
        /// <param name="proxyId"></param>
        /// <param name="saveCookies"></param>
        /// <param name="cookiesDays"></param>
        /// <param name="op_date"></param>
        /// <param name="operater"></param>
        /// <param name="isUseFParam"></param>
        /// <param name="fParamFormat"></param>
        /// <param name="orderFromType"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        [Ajax]
        public OperationStatus UpdateOrderFrom(string pkId, string name, string parentId, string coefficient, string intro, string status, string priority,
                    string makeComeFrom, string proxyId, string saveCookies, string cookiesDays, string op_date, string operater, string isUseFParam,
                    string fParamFormat, string orderFromType, string notes)
        {
            if (String.IsNullOrEmpty(operater))
            {
                //try
                //{
                    operater = ServiceFactory.MSUser.GetMSUser(CurrentMSUser.CardNo).UserName;
                //}
                //catch (Exception e)
                //{
                    //WebLog.ChannelLog.CommonLogger.Error("获取当前登录用户名异常：" + e.Message, e);
                //}
            }
            OrderFrom orderFrom = BuildOrderFrom(pkId, name, parentId, coefficient, intro, status, priority,
                    makeComeFrom, proxyId, saveCookies, cookiesDays, DateTime.Now.ToString(), operater, isUseFParam,
                    fParamFormat, orderFromType, notes);
            try
            {
                orderFromManageFacade.UpdateOrderFrom(orderFrom);
                return OperationStatus.Success;
            }
            catch (Exception e) {
                WebLog.ChannelLog.CommonLogger.Error("更新OrderFrom记录异常：" + e.Message, e);
                return OperationStatus.Error;
            }
        }

        /// <summary>
        /// 更新OrderFrom_Determine
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="categoryId"></param>
        /// <param name="sHostName"></param>
        /// <param name="sParamRegex"></param>
        /// <param name="tHostName"></param>
        /// <param name="tParamRegex"></param>
        /// <param name="priority"></param>
        /// <param name="op_date"></param>
        /// <param name="operater"></param>
        /// <returns></returns>
        [Ajax]
        public OperationStatus UpdateOrderFrom_Determine(string pkid, string categoryId, string sHostName,
                            string sParamRegex, string tHostName, string tParamRegex, string priority, string op_date, string operater)
        {
            if (String.IsNullOrEmpty(operater))
            {
                //try
                //{
                operater = ServiceFactory.MSUser.GetMSUser(CurrentMSUser.CardNo).UserName;
                //}
                //catch (Exception e)
                //{
                //    WebLog.ChannelLog.CommonLogger.Error("获取当前登录用户名异常：" + e.Message, e);
                //}
            }
            OrderFrom_Determine orderFrom_Determine = BuildOrderFrom_Determine(pkid, categoryId, sHostName,
                            sParamRegex, tHostName, tParamRegex, priority, DateTime.Now.ToString(), operater);

            try
            {
                orderFromDetermineFacade.UpdateOrderFrom_Determine(orderFrom_Determine);
                return OperationStatus.Success;
            }
            catch (Exception e)
            {
                WebLog.ChannelLog.CommonLogger.Error("更新OrderFrom_Determine记录异常：" + e.Message, e);
                return OperationStatus.Error;
            }
        }

        /// <summary>
        /// 依据pkid删除OrderFrom
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        [Ajax]
        public OperationStatus DeleteOrderFrom(string pkid)
        {
            OrderFrom orderFrom = new OrderFrom();
            orderFrom.Pkid = Convert.ToInt32(pkid);
            if (!IsHasFuncTypeName(CurrentMSUser.MSRoleIds, EFuncTypeName.OrderFrom_Operation))
            {
                return OperationStatus.NoPrivilegeError;
            }
            try
            {
                orderFromManageFacade.DeleteOrderFrom(orderFrom);
                return OperationStatus.Success;
            }
            catch (Exception e) {
                WebLog.ChannelLog.CommonLogger.Error("删除OrderFrom记录异常：" + e.Message, e);
                return OperationStatus.Error;
            }
        }

        /// <summary>
        /// 依据pkid删除OrderFrom_Determine
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        [Ajax]
        public OperationStatus DeleteOrderFrom_Determine(string pkid)
        {
            OrderFrom_Determine orderFrom_Determine = new OrderFrom_Determine();
            orderFrom_Determine.Pkid = Convert.ToInt32(pkid);
            if (!IsHasFuncTypeName(CurrentMSUser.MSRoleIds, EFuncTypeName.OrderFrom_Operation))
            {
                return OperationStatus.NoPrivilegeError;
            }
            try
            {
                orderFromDetermineFacade.DeleteOrderFrom_Determine(orderFrom_Determine);
                return OperationStatus.Success;
            }
            catch (Exception e)
            {
                WebLog.ChannelLog.CommonLogger.Error("删除OrderFrom_Determine记录异常：" + e.Message, e);
                return OperationStatus.Error;
            }
        }

        /// <summary>
        /// 获取OrderFrom_Determine页面的展示列表
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="sHostName"></param>
        /// <param name="sParamRegex"></param>
        /// <param name="tHostName"></param>
        /// <param name="tParamRegex"></param>
        /// <param name="categoryId"></param>
        /// <param name="priority"></param>
        /// <param name="operater"></param>
        /// <param name="op_date"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Ajax]
        public ActionResult GetOrderFromDetermineChart(string pkid, string categoryId, string sHostName,
                            string sParamRegex, string tHostName, string tParamRegex, string priority, string op_date,
                            string operater, string orderBy, int pageIndex, int pageSize)
        {
            OrderFromDetermineChartViewModel orderFromDetermineChartViewModel = new OrderFromDetermineChartViewModel();
            OrderFrom_Determine orderFrom_Determine = BuildOrderFrom_Determine(pkid, categoryId, sHostName,
                            sParamRegex, tHostName, tParamRegex, priority, op_date, operater);
            EntityTable<OrderFrom_Determine> entityOrderFrom_Determine = orderFromDetermineFacade.GetOrderFrom_DetermineList(orderFrom_Determine, orderBy, pageIndex, pageSize);
            orderFromDetermineChartViewModel.OrderFromDetermineListViewModel = entityOrderFrom_Determine.ToList();
            int recordCount = GetOrderFrom_DetermineListCount(pkid, categoryId, sHostName, sParamRegex,
                                                      tHostName, tParamRegex, priority, op_date, operater);
            orderFromDetermineChartViewModel.PageSize = pageSize;
            if (recordCount % orderFromDetermineChartViewModel.PageSize == 0)
            {
                orderFromDetermineChartViewModel.PageCount = recordCount / orderFromDetermineChartViewModel.PageSize;
            }
            else
            {
                orderFromDetermineChartViewModel.PageCount = recordCount / orderFromDetermineChartViewModel.PageSize + 1;
            }
            orderFromDetermineChartViewModel.PageIndex = pageIndex;
            if (pageIndex > 5)
            {
                orderFromDetermineChartViewModel.StartPage = pageIndex - 4;
            }
            else
            {
                orderFromDetermineChartViewModel.StartPage = 1;
            }
            if ((pageIndex == orderFromDetermineChartViewModel.PageCount) && (recordCount % orderFromDetermineChartViewModel.PageSize != 0))
            {
                orderFromDetermineChartViewModel.PageSize = recordCount % orderFromDetermineChartViewModel.PageSize;
            }
            return View("OrderFromDetermineChart", orderFromDetermineChartViewModel);
        }


        /// <summary>
        /// 模糊匹配获取OrderFrom_DetermineManage页面的展示列表
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="sHostName"></param>
        /// <param name="sParamRegex"></param>
        /// <param name="tHostName"></param>
        /// <param name="tParamRegex"></param>
        /// <param name="categoryId"></param>
        /// <param name="priority"></param>
        /// <param name="operater"></param>
        /// <param name="op_date"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Ajax]
        public ActionResult GetOrderFromDetermineChartFuzzy(string pkid, string categoryId, string sHostName,
                            string sParamRegex, string tHostName, string tParamRegex, string priority, string op_date, string operater, string orderBy, int pageIndex, int pageSize)
        {
            OrderFromDetermineChartViewModel orderFrom_DetermineChartVM = new OrderFromDetermineChartViewModel();
            OrderFrom_Determine orderFrom_Determine = BuildOrderFrom_Determine(pkid, categoryId, sHostName,
                            sParamRegex, tHostName, tParamRegex, priority, op_date, operater);
            List<OrderFrom_Determine> listOrderFrom_Determine = orderFromDetermineFacade.GetOrderFrom_DetermineListFuzzy(orderFrom_Determine, orderBy, pageIndex, pageSize);
            orderFrom_DetermineList = listOrderFrom_Determine.ToList();
            int recordCount = orderFrom_DetermineList.Count;
            orderFrom_DetermineChartVM.RecordCount = recordCount;
            orderFrom_DetermineChartVM.PageSize = pageSize;
            int frontPageSize = pageSize;
            if (recordCount % orderFrom_DetermineChartVM.PageSize == 0)
            {
                orderFrom_DetermineChartVM.PageCount = recordCount / orderFrom_DetermineChartVM.PageSize;
            }
            else
            {
                orderFrom_DetermineChartVM.PageCount = recordCount / orderFrom_DetermineChartVM.PageSize + 1;
            }
            orderFrom_DetermineChartVM.PageIndex = pageIndex;
            if (pageIndex > 5)
            {
                orderFrom_DetermineChartVM.StartPage = pageIndex - 4;
            }
            else
            {
                orderFrom_DetermineChartVM.StartPage = 1;
            }
            if ((pageIndex == orderFrom_DetermineChartVM.PageCount) && (recordCount % orderFrom_DetermineChartVM.PageSize != 0))
            {
                orderFrom_DetermineChartVM.PageSize = recordCount % orderFrom_DetermineChartVM.PageSize;
            }
            if (recordCount == 0)
            {
                orderFrom_DetermineChartVM.PageSize = 0;
            }
            orderFrom_DetermineChartVM.OrderFromDetermineListViewModel = new List<OrderFrom_Determine>();
            for (int i = 0; i < orderFrom_DetermineChartVM.PageSize; i++)
            {
                orderFrom_DetermineChartVM.OrderFromDetermineListViewModel.Add(orderFrom_DetermineList[(orderFrom_DetermineChartVM.PageIndex - 1) * frontPageSize + i]);
            }
            return View("OrderFromDetermineChart", orderFrom_DetermineChartVM);
        }

        /// <summary>
        /// 通过pkid获得匹配项的name
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        [Ajax]
        public string GetOrderFromNameByPkid(string pkid)
        {
            int intPkid = Convert.ToInt32(pkid);
            string ret = orderFromManageFacade.GetName(intPkid);
            return ret;
        }

        /// <summary>
        /// 获取OrderFromManage页面的展示列表
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="name"></param>
        /// <param name="parentId"></param>
        /// <param name="coefficient"></param>
        /// <param name="intro"></param>
        /// <param name="status"></param>
        /// <param name="priority"></param>
        /// <param name="makeComeFrom"></param>
        /// <param name="proxyId"></param>
        /// <param name="saveCookies"></param>
        /// <param name="cookiesDays"></param>
        /// <param name="operater"></param>
        /// <param name="op_date"></param>
        /// <param name="notes"></param>
        /// <param name="isUseFParam"></param>
        /// <param name="fParamFormat"></param>
        /// <param name="orderFromType"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Ajax]
        public ActionResult GetOrderFromManageChart(string pkid, string name, string parentId, string coefficient, string intro, string status, string priority,
                    string makeComeFrom, string proxyId, string saveCookies, string cookiesDays, string op_date, string operater, string isUseFParam,
                    string fParamFormat, string orderFromType, string notes, string orderBy, int pageIndex, int pageSize)
        {
            OrderFromManageChartViewModel orderFromManageChartVM = new OrderFromManageChartViewModel();
            intro = "";
            makeComeFrom = "";
            saveCookies = "";
            isUseFParam = "";
            OrderFrom orderFrom = BuildOrderFrom(pkid, name, parentId, coefficient, intro, status, priority, makeComeFrom, proxyId,
                                saveCookies, cookiesDays, op_date, operater, isUseFParam, fParamFormat, orderFromType, notes);
            EntityTable<OrderFrom> entityOrderFrom = orderFromManageFacade.GetOrderFromList(orderFrom, orderBy, pageIndex, pageSize);
            orderFromManageChartVM.OrderFromManageListViewModel = entityOrderFrom.ToList();
            int count = orderFromManageChartVM.OrderFromManageListViewModel.Count;
            int recordCount = GetOrderFromListCount(pkid, name, parentId, coefficient, intro, status, priority, makeComeFrom, proxyId,
                                saveCookies, cookiesDays, op_date, operater, isUseFParam, fParamFormat, orderFromType, notes);
            orderFromManageChartVM.RecordCount = recordCount;
            orderFromManageChartVM.PageSize = pageSize;
            if (recordCount % orderFromManageChartVM.PageSize == 0)
            {
                orderFromManageChartVM.PageCount = recordCount / orderFromManageChartVM.PageSize;
            }
            else
            {
                orderFromManageChartVM.PageCount = recordCount / orderFromManageChartVM.PageSize + 1;
            }
            orderFromManageChartVM.PageIndex = pageIndex;
            if (pageIndex > 5)
            {
                orderFromManageChartVM.StartPage = pageIndex - 4;
            }
            else
            {
                orderFromManageChartVM.StartPage = 1;
            }
            if ((pageIndex == orderFromManageChartVM.PageCount) && (recordCount % orderFromManageChartVM.PageSize != 0))
            {
                orderFromManageChartVM.PageSize = recordCount % orderFromManageChartVM.PageSize;
            }
            return View("OrderFromManageChart", orderFromManageChartVM);
        }

        /// <summary>
        /// 模糊匹配获取OrderFromManage页面的展示列表
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="name"></param>
        /// <param name="parentId"></param>
        /// <param name="coefficient"></param>
        /// <param name="intro"></param>
        /// <param name="status"></param>
        /// <param name="priority"></param>
        /// <param name="makeComeFrom"></param>
        /// <param name="proxyId"></param>
        /// <param name="saveCookies"></param>
        /// <param name="cookiesDays"></param>
        /// <param name="operater"></param>
        /// <param name="op_date"></param>
        /// <param name="notes"></param>
        /// <param name="isUseFParam"></param>
        /// <param name="fParamFormat"></param>
        /// <param name="orderFromType"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Ajax]
        public ActionResult GetOrderFromManageChartFuzzy(string pkid, string name, string parentId, string coefficient, string intro, string status, string priority,
                    string makeComeFrom, string proxyId, string saveCookies, string cookiesDays, string op_date, string operater, string isUseFParam,
                    string fParamFormat, string orderFromType, string notes, string orderBy, int pageIndex, int pageSize)
        {
            OrderFromManageChartViewModel orderFromManageChartVM = new OrderFromManageChartViewModel();
            intro = "";
            makeComeFrom = "";
            saveCookies = "";
            isUseFParam = "";
            OrderFrom orderFrom = BuildOrderFrom(pkid, name, parentId, coefficient, intro, status, priority, makeComeFrom, proxyId,
                                saveCookies, cookiesDays, op_date, operater, isUseFParam, fParamFormat, orderFromType, notes);
            List<OrderFrom> listOrderFrom = orderFromManageFacade.GetOrderFromListFuzzy(orderFrom, orderBy, pageIndex, pageSize);
            orderFromManageList = listOrderFrom.ToList();
            int recordCount = orderFromManageList.Count;
            orderFromManageChartVM.RecordCount = recordCount;
            orderFromManageChartVM.PageSize = pageSize;
            int frontPageSize = pageSize;
            if (recordCount % orderFromManageChartVM.PageSize == 0)
            {
                orderFromManageChartVM.PageCount = recordCount / orderFromManageChartVM.PageSize;
            }
            else
            {
                orderFromManageChartVM.PageCount = recordCount / orderFromManageChartVM.PageSize + 1;
            }

            orderFromManageChartVM.PageIndex = pageIndex;

            if (pageIndex > 5)
            {
                orderFromManageChartVM.StartPage = pageIndex - 4;
            }
            else
            {
                orderFromManageChartVM.StartPage = 1;
            }

            if ((pageIndex == orderFromManageChartVM.PageCount) && (recordCount % orderFromManageChartVM.PageSize != 0))
            {
                orderFromManageChartVM.PageSize = recordCount % orderFromManageChartVM.PageSize;
            }

            if (recordCount == 0)
            {
                orderFromManageChartVM.PageSize = 0;
            }

            orderFromManageChartVM.OrderFromManageListViewModel = new List<OrderFrom>();
            for (int i = 0; i < orderFromManageChartVM.PageSize; i++)
            {
                orderFromManageChartVM.OrderFromManageListViewModel.Add(orderFromManageList[(orderFromManageChartVM.PageIndex - 1) * frontPageSize + i]);
            }
            return View("OrderFromManageChart", orderFromManageChartVM);
        }

        #endregion

        #region public function
        /// <summary>
        /// 获得OrderFrom_Determine中展示列表的总记录数
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="sHostName"></param>
        /// <param name="sParamRegex"></param>
        /// <param name="tHostName"></param>
        /// <param name="tParamRegex"></param>
        /// <param name="categoryId"></param>
        /// <param name="priority"></param>
        /// <param name="operater"></param>
        /// <param name="op_date"></param>
        /// <returns></returns>
        public int GetOrderFrom_DetermineListCount(string pkid, string categoryId, string sHostName, string sParamRegex,
                                                    string tHostName, string tParamRegex, string priority, string op_date, string operater)
        {
            OrderFrom_Determine orderFrom_Determine = BuildOrderFrom_Determine(pkid, categoryId, sHostName, sParamRegex,
                                                    tHostName, tParamRegex, priority, op_date, operater);

            return orderFromDetermineFacade.GetOrderFrom_DetermineListCount(orderFrom_Determine);
        }

        /// <summary>
        /// 获得OrderFrom中展示列表的总记录数
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="name"></param>
        /// <param name="parentId"></param>
        /// <param name="coefficient"></param>
        /// <param name="intro"></param>
        /// <param name="status"></param>
        /// <param name="priority"></param>
        /// <param name="makeComeFrom"></param>
        /// <param name="proxyId"></param>
        /// <param name="saveCookies"></param>
        /// <param name="cookiesDays"></param>
        /// <param name="operater"></param>
        /// <param name="op_date"></param>
        /// <param name="notes"></param>
        /// <param name="isUseFParam"></param>
        /// <param name="fParamFormat"></param>
        /// <param name="orderFromType"></param>
        /// <returns></returns>
        public int GetOrderFromListCount(string pkid, string name, string parentId, string coefficient, string intro, string status, string priority,
                    string makeComeFrom, string proxyId, string saveCookies, string cookiesDays, string op_date, string operater, string isUseFParam,
                    string fParamFormat, string orderFromType, string notes)
        {
            OrderFrom orderFrom = BuildOrderFrom(pkid, name, parentId, coefficient, intro, status, priority, makeComeFrom, proxyId,
                                saveCookies, cookiesDays, op_date, operater, isUseFParam, fParamFormat, orderFromType, notes);

            return orderFromManageFacade.GetOrderFromListCount(orderFrom);
        }

        

        /// <summary>
        /// 通过传入的9各参数，构造一个OrderFrom_Determine实例并复制给函数外部的一个变量(用于插入，update等，不同于查询)
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="categoryId"></param>
        /// <param name="sHostName"></param>
        /// <param name="sParamRegex"></param>
        /// <param name="tHostName"></param>
        /// <param name="tParamRegex"></param>
        /// <param name="priority"></param>
        /// <param name="op_date"></param>
        /// <param name="operater"></param>
        /// <param name="orderFrom_Determine"></param>
        public OrderFrom_Determine BuildOrderFrom_Determine(string pkid, string categoryId, string sHostName, string sParamRegex,
                                                    string tHostName, string tParamRegex, string priority, string op_date, string operater)
        {
            OrderFrom_Determine orderFrom_Determine2 = new OrderFrom_Determine();
            if (!String.IsNullOrEmpty(pkid))
            {
                orderFrom_Determine2.Pkid = Convert.ToInt32(pkid);
            }
            if (!String.IsNullOrEmpty(categoryId))
            {
                orderFrom_Determine2.CategoryId = Convert.ToInt32(categoryId);
            }

            orderFrom_Determine2.SHostName = sHostName;
            orderFrom_Determine2.SParamRegex = sParamRegex;
            orderFrom_Determine2.THostName = tHostName;
            orderFrom_Determine2.TParamRegex = tParamRegex;

            if (!String.IsNullOrEmpty(priority))
            {
                orderFrom_Determine2.Priority = Convert.ToInt32(priority);
            }
            if (!String.IsNullOrEmpty(op_date))
            {
                orderFrom_Determine2.Op_date = Convert.ToDateTime(op_date);
            }
            orderFrom_Determine2.Operator = operater;
            return orderFrom_Determine2;
        }

        /// <summary>
        /// 通过传入的17各参数，构造一个OrderFrom_Determine实例并复制给函数外部的一个变量(用于查询)
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="name"></param>
        /// <param name="parentId"></param>
        /// <param name="coefficient"></param>
        /// <param name="intro"></param>
        /// <param name="status"></param>
        /// <param name="priority"></param>
        /// <param name="makeComeFrom"></param>
        /// <param name="proxyId"></param>
        /// <param name="saveCookies"></param>
        /// <param name="cookiesDays"></param>
        /// <param name="op_date"></param>
        /// <param name="operater"></param>
        /// <param name="isUseFParam"></param>
        /// <param name="fParamFormat"></param>
        /// <param name="orderFromType"></param>
        /// <param name="notes"></param>
        /// <param name="orderFrom2"></param>
        public OrderFrom BuildOrderFrom(string pkid, string name, string parentId, string coefficient, string intro, string status, string priority,
                    string makeComeFrom, string proxyId, string saveCookies, string cookiesDays, string op_date, string operater, string isUseFParam,
                    string fParamFormat, string orderFromType, string notes)
        {
            OrderFrom orderFrom = new OrderFrom();

            if (!String.IsNullOrEmpty(pkid))
            {
                orderFrom.Pkid = Convert.ToInt32(pkid);
            }
            orderFrom.Name = name;
            if (!String.IsNullOrEmpty(parentId))
            {
                orderFrom.ParentId = Convert.ToInt32(parentId);
            }
            if (!String.IsNullOrEmpty(coefficient))
            {
                orderFrom.Coefficient = Convert.ToSingle(coefficient);
            }
            orderFrom.Intro = intro;
            if (!String.IsNullOrEmpty(status))
            {
                orderFrom.Status = Convert.ToInt32(status);
            }
            if (!String.IsNullOrEmpty(priority))
            {
                orderFrom.Priority = Convert.ToInt32(priority);
            }
            if (!String.IsNullOrEmpty(makeComeFrom))
            {
                orderFrom.MakeComeFrom = Convert.ToInt32(makeComeFrom);
            }
            orderFrom.ProxyId = proxyId;
            if (!String.IsNullOrEmpty(saveCookies))
            {
                orderFrom.SaveCookies = Convert.ToInt32(saveCookies);
            }
            if (!String.IsNullOrEmpty(cookiesDays))
            {
                orderFrom.CookiesDays = Convert.ToInt32(cookiesDays);
            }
            if (!String.IsNullOrEmpty(op_date))
            {
                orderFrom.Op_date = Convert.ToDateTime(op_date);
            }
            orderFrom.Operator = operater;
            if (!String.IsNullOrEmpty(isUseFParam))
            {
                orderFrom.IsUseFParam = Convert.ToInt16(isUseFParam);
            }
            orderFrom.FParamFormat = fParamFormat;
            orderFrom.OrderFromType = orderFromType;
            orderFrom.Notes = notes;

            return orderFrom;

        }

        /// <summary>
        /// 通过categoryId来获得匹配的记录
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public EntityTable<OrderFrom_Determine> GetOrderFrom_DetermineByCategoryId(OrderFrom_Determine orderFrom_Determine)
        {
            return orderFromDetermineFacade.GetOrderFrom_DetermineByOrderFromId(orderFrom_Determine);
        }
        #endregion
    }
}
