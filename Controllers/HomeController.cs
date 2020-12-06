using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ECommerce.Models;

namespace ECommerce.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            IEnumerable<ProductViewModel> objProduct = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Convert.ToString(ConfigurationManager.AppSettings["APIURL"]));
                //HTTP GET
                var responseTask = client.GetAsync("GetAllProducts");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ProductViewModel>>();
                    readTask.Wait();

                    objProduct = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    objProduct = Enumerable.Empty<ProductViewModel>();

                    ModelState.AddModelError(string.Empty, "Server error in API. Please contact administrator.");
                }
            }

            ViewBag.APIURL = Convert.ToString(ConfigurationManager.AppSettings["APIURL"]);
            return View(objProduct);
        }

        [HttpGet]
        public ActionResult Create(int? ProductId)
        {
            ProductViewModel objProduct = new ProductViewModel();

            //Get Category list
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Convert.ToString(ConfigurationManager.AppSettings["APIURL"]));
                //HTTP GET
                var responseTask = client.GetAsync("GetAllCategories");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ProductViewModel>>();
                    readTask.Wait();

                    var CategoryList = readTask.Result;
                    ViewBag.CategoryList = new SelectList(CategoryList, "ProdCatId", "CategoryName");

                    if (ProductId != 0) //Edit Mode load all values
                    {
                        responseTask = client.GetAsync("GetAllProducts?Id=" + ProductId);
                        responseTask.Wait();

                        result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTaskEdit = result.Content.ReadAsAsync<ProductViewModel>();
                            readTaskEdit.Wait();

                            objProduct = readTaskEdit.Result;
                        }
                    }
                }
                else //web api sent error response 
                {
                    ViewBag.CategoryList = new List<SelectListItem>();
                    ModelState.AddModelError(string.Empty, "Server error in API. Please contact administrator.");
                }

                
            }

            return View(objProduct);
        }

        [HttpPost]
        public ActionResult Create(ProductViewModel objProduct, FormCollection Form)
        {
            //ProductViewModel objProduct = new ProductViewModel();
            List<ProductAttributeViewModel> icollection = new List<ProductAttributeViewModel>();
            //objProduct.ListProductAttributes = new IList<ProductAttributeViewModel>();

            //Get Category list
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Convert.ToString(ConfigurationManager.AppSettings["APIURL"]));

                //HTTP GET
                var responseTask = client.GetAsync("GetAllCategories");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ProductViewModel>>();
                    readTask.Wait();

                    var CategoryList = readTask.Result;
                    ViewBag.CategoryList = new SelectList(CategoryList, "ProdCatId", "CategoryName");
                }
                else //web api sent error response 
                {
                    //log response status here..

                    //objProduct = Enumerable.Empty<ProductViewModel>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }

                // ================== Get Product Attributes
                responseTask = client.GetAsync("GetAllProductAttributeLookup?ProdCatId=" + objProduct.ProdCatId.ToString() + "&ProductId=" + objProduct.ProductId);
                responseTask.Wait();

                result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ProductAttributeViewModel>>();
                    readTask.Wait();

                    var AttributeList = readTask.Result;
                    foreach (var item in AttributeList)
                    {
                        int AttributeId = item.AttributeId;
                        string id = "txtAttribute_" + AttributeId;
                        string AttributeValue = Form[id];

                        ProductAttributeViewModel objAttribute = new ProductAttributeViewModel();
                        objAttribute.AttributeId = AttributeId;
                        objAttribute.AttributeValue = AttributeValue;
                        icollection.Add(objAttribute);

                    }

                    objProduct.ListProductAttributes = icollection;
                }
                //========================

                //Save Record
                if (ModelState.IsValid)
                {
                    if (objProduct.ProductId == 0)
                    {
                        var postTask = client.PostAsJsonAsync<ProductViewModel>("PostNewProduct", objProduct);
                        postTask.Wait();

                        result = postTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record inserted successfully.";
                            return RedirectToAction("Index");
                        }
                        else if(result.StatusCode == System.Net.HttpStatusCode.Conflict) //Conflict : 409
                        {
                            ModelState.AddModelError(string.Empty, "Record already exists.");
                            return View(objProduct);
                        }
                    }
                    else
                    {
                        var putTask = client.PutAsJsonAsync<ProductViewModel>("PutProduct", objProduct);
                        putTask.Wait();

                        result = putTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            TempData["Message"] = "Record updated successfully.";
                            return RedirectToAction("Index");
                        }
                        else if (result.StatusCode == System.Net.HttpStatusCode.Conflict) //Conflict : 409
                        {
                            ModelState.AddModelError(string.Empty, "Record already exists.");
                            return View(objProduct);
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
                }
            }

            return View(objProduct);
        }

        public ActionResult About()
        {
            ViewBag.Message = "E Commerce demo application.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Detail.";

            return View();
        }

        public ActionResult GetAttributes(int ProdCatId, long? ProductId)
        {
            IList<ProductAttributeLookupViewModel> model = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Convert.ToString(ConfigurationManager.AppSettings["APIURL"]));
                //HTTP GET
                var responseTask = client.GetAsync("GetAllProductAttributeLookup?ProdCatId=" + ProdCatId + "&ProductId=" + ProductId);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ProductAttributeLookupViewModel>>();
                    readTask.Wait();

                    model = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    //objProduct = Enumerable.Empty<ProductViewModel>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            //for (int i = 0; i < model.Count(); i++)
            //{
            //    //model[i].AttributeValue = "test" + i;
            //    var a = model[i].AttributeValue;
            //}

            return PartialView("_ProductAttribute", model);
        }

        public ActionResult DeleteProduct(int ProductId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Convert.ToString(ConfigurationManager.AppSettings["APIURL"]));
                //HTTP GET
                var deleteTask = client.DeleteAsync("DeleteProduct?ProductId=" + ProductId);
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return Json(new { returnStr = "Success" });
                }
                else //web api sent error response 
                {
                    //log response status here..
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    return Json(new { returnStr = "Server Error" });
                }
            }

            //return RedirectToAction("Index");
        }
    }
}