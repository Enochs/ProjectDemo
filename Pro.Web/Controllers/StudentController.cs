﻿using Pro.Dal.Stu;
using Pro.Model;
using Pro.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Pro.Common;
using System.Web.Mvc;
using Pro.Extension;
using Pro.Repository.Repository;
using Pro.Dal.Constellatory;
using Pro.Dal.Base;
using Pro.Model.model;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Pro.Web.Controllers
{
    public class StudentController : BaseController
    {
        #region 构造函数
        /// <summary>
        /// 构造函数  声明 EF
        /// </summary>
        /// 
        private IStudentService stuService;
        private IConstellationService consService;
        private IBaseService<Student> baseService;
        private IDataRepository<Student> stuReporitory;
        private IBaseService<Company> comService;
        public StudentController(IStudentService _stuService, IConstellationService _consService, IBaseService<Student> _baseService, IDataRepository<Student> _stuReporitory, IBaseService<Company> _comService)
        {
            this.stuService = _stuService;
            this.consService = _consService;
            this.baseService = _baseService;
            this.stuReporitory = _stuReporitory;
            this.comService = _comService;
        }
        #endregion


        #region 学生列表
        /// <summary>
        /// 获取学生列表
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult StuList(string stuName, int page = 1)
        {
            string pageSzie = Request["pageSize"];

            var dataList = stuService.GetAllStu(stuName, "", "", page, 10);
            return View(dataList);
        }


        /// <summary>
        /// 分布视图 异步刷新
        /// </summary>
        /// <param name="stuName"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult List(string stuName, string Status, string sort, string pagesize, int page = 1)
        {
            #region 下拉框

            Enum status = SysStatus.Enable;
            ViewData["selectList"] = status.GetSelectList();

            #endregion

            if (string.IsNullOrEmpty(pagesize))
            {
                pagesize = "5";
            }
            if (string.IsNullOrEmpty(sort))
            {
                sort = "s_createDate-desc";
            }

            var dataList = stuService.GetAllStu(stuName, Status, sort, page, pagesize.ToInt32());

            if (Request.IsAjaxRequest())
            {
                return PartialView("_StuList", dataList);
            }
            return View(dataList);
        }
        #endregion

        #region 学生编辑页面
        /// <summary>
        /// 编辑页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            #region 下拉框

            Enum gender = Gender.Male;
            ViewData["selectList"] = gender.GetSelectList(false);

            Enum grade = GradeName.FirstGrade;
            ViewData["Grade"] = grade.GetSelectList(false);
            #endregion

            EFDbContext _context = new EFDbContext();
            Student stu = new Student();
            if (id != 0)
            {
                //stu = stuService.GetModel(id);
                stu = baseService.GetById(id);
            }
            return View(stu);
        }

        /// <summary>
        /// 编辑页面  修改功能
        /// </summary>
        /// <param name="stu"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateStu(Student stu)
        {
            AjaxMessage ajax = new AjaxMessage();
            ajax.IsSuccess = false;
            ajax.Message = "系统异常";
            if (stu != null)
            {
                Student result = stuService.ModifyStudent(stu);
                if (result != null)
                {
                    ajax.IsSuccess = true;
                    ajax.Message = "修改成功";
                }

            }
            return Json(ajax);
        }
        #endregion

        #region 修改状态
        // <summary>
        /// 编辑页面  修改状态
        /// </summary>
        /// <param name="stu"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateStatus(string id, int status)
        {
            AjaxMessage ajax = new AjaxMessage();
            ajax.IsSuccess = false;
            ajax.Message = "系统异常";
            if (!string.IsNullOrEmpty(id))
            {
                int result = stuService.ModifyStatus(id, status);
                if (result > 0)
                {
                    ajax.IsSuccess = true;
                    if (status == 1)
                    {
                        ajax.Message = "启用成功";
                    }
                    else
                    {
                        ajax.Message = "禁用成功";
                    }
                }

            }
            return Json(ajax);
        }
        #endregion

        #region 学生添加页面
        /// <summary>
        /// 添加页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Insert()
        {
            #region 下拉框

            Enum gender = Gender.Male;
            ViewData["selectList"] = gender.GetSelectList(false);

            Enum grade = GradeName.FirstGrade;
            ViewData["Grade"] = grade.GetSelectList(false);

            #endregion

            return View();
        }

        /// <summary>
        /// 添加页面  功能实现
        /// </summary>
        /// <param name="stu"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddStudent(Student stu)
        {
            AjaxMessage ajax = new AjaxMessage();
            ajax.IsSuccess = false;
            ajax.Message = "系统异常";
            if (stu != null)
            {
                stu.s_status = 1;
                stu.s_createDate = DateTime.Now;
                Student result = baseService.Add(stu);
                if (result != null)
                {
                    ajax.IsSuccess = true;
                    ajax.Message = "添加成功";
                }

            }
            return Json(ajax);
        }
        #endregion

        #region 查看详细
        /// <summary>
        /// 详细信息
        /// </summary>
        public JsonResult GetDetails(string id)
        {

            string res = "";
            if (!string.IsNullOrEmpty(id))
            {
                Student student = stuService.GetModel(id.ToInt32());
                if (student != null)
                {
                    res += (@"<div class='form-group'><table class='table table-bordered table-striped table-hover'><tr><td>姓名:</td><td>" + student.s_name + @"</td></tr>
                        <tr><td>年龄:</td><td>" + student.s_age + @"</td></tr>
                        <tr><td>性别:</td><td>" + (student.s_sex.ToString() == "False" ? "男" : "女") + @"</td></tr>
                        <tr><td>地址:</td><td>" + student.s_address + @"</td></tr>
                        <tr><td>联系电话:</td><td>" + student.s_phone + @"</td></tr>
                        <tr><td>创建日期:</td><td>" + student.s_createDate + @"</td></tr>
                        <tr><td>状态:</td><td>" + (student.s_status.ToString() == "1" ? "启用" : "禁用") + @"</td></tr>
                        </table></div>");
                }
            }
            return Json(res);
        }

        #endregion

        #region 根据ID删除学生
        /// <summary>
        /// 根据ID删除学生
        /// </summary>
        /// <returns></returns>
        public JsonResult DeleteStu(string stu_id)
        {
            AjaxMessage ajax = new AjaxMessage();
            ajax.IsSuccess = false;
            ajax.Message = "系统异常";
            if (!string.IsNullOrEmpty(stu_id))
            {
                int Id = stu_id.ToInt32();
                Expression<Func<Student, bool>> parm = c => c.s_id == Id;
                int result = baseService.DelBy(parm);
                if (result > 0)
                {
                    ajax.IsSuccess = true;
                    ajax.Message = "删除成功";
                }
                else
                {
                    ajax.Message = "删除失败,请稍后再试";
                }
            }
            return Json(ajax);
        }
        #endregion


        #region 树形视图
        /// <summary>
        /// 树形视图
        /// </summary>
        /// <returns></returns>
        public ActionResult TreeIndex()
        {
            //var dataList = comService.GetListBy(null);
            //var tree = (from c in dataList
            //            select new TreeVO()
            //            {
            //                id = c.CompanyID,
            //                pid = c.PId,
            //                name = c.CompanyName
            //            });
            //string strResult = JsonConvert.SerializeObject(tree);
            //ViewData["data"] = strResult;
            EFDbContext ObjEntity = new EFDbContext();
            var stuList = ObjEntity.Student.ToList();
            var gradeList = ObjEntity.Grade.ToList();
            List<TreeVO> tree = new List<TreeVO>();
            foreach (var item in gradeList)
            {
                TreeVO entity = new TreeVO();
                entity.id = item.GradeID;
                entity.pid = 0;
                entity.name = item.GradeName;
                tree.Add(entity);
            }

            foreach (var item in stuList)
            {
                TreeVO entity = new TreeVO();
                if (item.s_id >= 4)
                {
                    entity.id = item.s_id;
                    entity.pid = item.s_GradeID;
                    entity.name = item.s_name;
                    tree.Add(entity);
                }
            }
            //var tree = (from c in ObjEntity.Grade
            //            join d in ObjEntity.Student on c.GradeID equals d.s_GradeID
            //            select new TreeVO()
            //            {
            //                id = d.s_id,
            //                pid = c.GradeID,
            //                name = d.s_name
            //            });
            string strResult = JsonConvert.SerializeObject(tree);
            ViewData["data"] = strResult;
            return View();
        }
        #endregion

        #region 获取所有公司名称
        /// <summary>
        /// 获取所有公司名称
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllTree()
        {
            var dataList = comService.GetListBy(null);
            var tree = (from c in dataList
                        select new TreeVO()
                        {
                            id = c.CompanyID,
                            pid = c.PId,
                            name = c.CompanyName
                        });
            return Json(tree);
        }
        #endregion


        #region Suggest页面
        /// <summary>
        /// suggest搜索页面
        /// </summary>
        /// <returns></returns>

        public ActionResult Suggest()
        {
            return View();
        }
        #endregion


        #region suggest获取学生信息
        /// <summary>
        /// 获取学生信息
        /// </summary>
        /// <returns></returns>

        public JsonResult GetPartialStu()
        {
            var json = Json(new { });
            try
            {
                using (EFDbContext ObjEntity = new EFDbContext())
                {
                    var stuList = ObjEntity.Student.ToList();
                    json = Json(new { value = stuList });
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            return json;
        }
        #endregion

    }
}