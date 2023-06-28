using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1Lesson3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Text;
using System.Drawing.Printing;

namespace WebApplication1Lesson3.Controllers
{
    public class HomeController : Controller
    {
        //*************************************   連結 MVC_UserDB 資料庫  ********************************* 
        private readonly MvcUserDbContext _db = new MvcUserDbContext();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, MvcUserDbContext context)
        {
            _logger = logger;
            _db = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateConfire(UserTable _userTable)
        {
            if(_userTable != null && ModelState.IsValid)
            {
                //_db.UserTables.Add(_userTable);
                //_db.SaveChanges();

                _db.Entry(_userTable).State = EntityState.Added;
                _db.SaveChanges();

                //return Content("Add one record success.");
                return RedirectToAction("List");
            }
            else
            {
                ModelState.AddModelError("Value1", "自訂錯誤訊息(1) "); // 第一個輸入值是 key，第二個是錯誤訊息（字串）
                ModelState.AddModelError("Value2", "自訂錯誤訊息(2) ");
                return View();
            }
        }

        [HttpGet]
        public IActionResult Delete(int? _ID)
        {
            if(_ID == null)
                return new StatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);

            IQueryable<UserTable> ListOne = _db.UserTables.Where(x => x.UserId == _ID);
            if (!ListOne.Any())
                return NotFound();
            else
                return View(ListOne.FirstOrDefault());

            //UserTable? ut = _db.UserTables.Find(_ID);
            //if(ut == null)
            //    return NotFound();
            //else
            //    return View(ut);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int? _ID)
        {
            if (ModelState.IsValid)
            {
                SqlParameter para = new SqlParameter("IDParameter", _ID);
                var ListOne = _db.UserTables.FromSqlRaw("Select * from USERTABLE with (nolock) where UserID=@IDParameter", para)
                                            .OrderBy(x => x.UserId)
                                            .FirstOrDefault();
                if (ListOne == null)
                    return Content(" 刪除時，找不到這一筆記錄！");
                else
                {
                    _db.Entry(ListOne).State = EntityState.Deleted;
                    _db.SaveChanges();
                }

                return RedirectToAction(nameof(List));

                //IQueryable<UserTable> ut = from m in _db.UserTables
                //                           where m.UserId == _ID
                //                           select m;
                //if(!ut.Any())
                //    return Content(" 刪除時，找不到這一筆記錄！");
                //else
                //{
                //    _db.UserTables.Remove(ut.First());
                //    _db.SaveChanges();

                //    return RedirectToAction(nameof(List));
                //}

                //var ut = _db.UserTables.Where(x => x.UserId == _ID);
                //if(!ut.Any())
                //    return Content(" 刪除時，找不到這一筆記錄！");
                //else
                //{
                //    _db.UserTables.Remove(ut.First());
                //    _db.SaveChanges();

                //    return RedirectToAction(nameof(List));
                //}
            }
            else
            {   // 搭配 ModelState.IsValid，如果驗證沒過，就出現錯誤訊息。
                ModelState.AddModelError("Value1", " 自訂錯誤訊息(1) ");
                ModelState.AddModelError("Value2", " 自訂錯誤訊息(2) ");
                return View();   // 將錯誤訊息，返回並呈現在「刪除」的檢視畫面上
            }
        }

        [HttpGet]
        public IActionResult Edit(int? _ID = 1)
        {
            if (!_ID.HasValue || _ID == null)
                return Content("Please enter UserID");

            IQueryable<UserTable> ListOne = from m in _db.UserTables
                                 where m.UserId == _ID
                                 select m;
            if (!ListOne.Any())
                return Content("Can't find UserID is " + _ID);
            else
                return View(ListOne.FirstOrDefault());
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public IActionResult EditConfirm([Bind("UserId, UserName, UserSex, UserBirthDay, UserMobilePhone")]UserTable _userTable)
        {
            if (_userTable == null)
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);

            if (ModelState.IsValid)
            {
                //_db.Entry(_userTable).State = EntityState.Modified;
                //_db.SaveChanges();

                _db.UserTables.Update(_userTable);
                _db.SaveChanges();

                return RedirectToAction(nameof(List));
            }
            else
            {
                return Content("Update fail.");
            }
        }

        public IActionResult Edit2(int? _ID = 1)
        {
            if (_ID == null)
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);

            UserTable? ut = _db.UserTables.Find(_ID);
            if(ut == null)
                return NotFound();
            else
                return View(ut);
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit2Confirm([Bind("UserId, UserName, UserSex, UserBirthDay, UserMobilePhone")]int? _ID)
        {
            if(string.IsNullOrEmpty(_ID.ToString()))
                  return new StatusCodeResult((int)HttpStatusCode.BadRequest);

            if (ModelState.IsValid)
            {
                if(await TryUpdateModelAsync<UserTable>(_db.UserTables.Find(_ID), "",
                                                        s=>s.UserName, s=>s.UserSex, s=>s.UserBirthDay, s => s.UserMobilePhone))
                {
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(List));
                    
                }
                else 
                    return Content(" *** 更新失敗！！*** ");
            }
            else
            {
                return Content(" *** ModelState.IsValid 驗證失敗！！*** ");
            }
        }

        public IActionResult IndexPage(int _ID = 1)
        {
            int PageSize = 5;
            int NowPageCnt = 0;
            if (_ID > 0 || string.IsNullOrEmpty(_ID.ToString()))
                NowPageCnt = (_ID - 1) * PageSize;

            var ListAll = (from m in _db.UserTables
                           orderby m.UserId
                           select m).Skip(NowPageCnt).Take(PageSize);

            if(!ListAll.Any())
                return NotFound();
            else
                return View(ListAll.ToList());
        }

        public IActionResult IndexPage2(int _ID = 1)
        {
            int PageSize = 5;
            int NowPageCnt = 0;

            if (_ID > 0 || string.IsNullOrEmpty(_ID.ToString()))
                NowPageCnt = (_ID - 1) * PageSize;

            var ListAll = (from m in _db.UserTables
                           orderby m.UserId
                           select m).Skip(NowPageCnt).Take(PageSize);
            if (!ListAll.Any())
                return NotFound();

            
            int remainder = _db.UserTables.Count() % PageSize;
            int Pages = _db.UserTables.Count() / PageSize;
            if (remainder > 0)
                Pages += 1;
            
            StringBuilder sbPageList = new StringBuilder();
            if(Pages > 0)
            {
                sbPageList.Append("<div align='center'>");
                sbPageList.Append("</div>");
            }

            if (_ID > 1)
                sbPageList.Append("<a href='?_ID=" + (_ID + 1) + "'>[下一頁>>>]</a>");
            sbPageList.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b><a href='http://127.0.0.1/'>[首頁]</a></b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
            if (_ID < Pages)
                sbPageList.Append("<a href='?_ID=" + (_ID - 1) + "'>[上一頁>>>]</a>");

            sbPageList.Append("<hr width='97%' size=1>");

            int block_page = 0;
            block_page = _ID / 10;   //--只取除法的整數成果（商），若有餘數也不去管它。

            if (block_page > 0)
            {
                sbPageList.Append("<a href='?_ID=" + (((block_page - 1) * 10) + 9) + "'> [前十頁<<]  </a>&nbsp;&nbsp;");
            }

            for (int K = 0; K <= 10; K++)
            {
                if ((block_page * 10 + K) <= Pages)
                {   //--- Pages 資料的總頁數。共需「幾頁」來呈現所有資料？
                    if (((block_page * 10) + K) == _ID)
                    {   //--- id 就是「目前在第幾頁」
                        sbPageList.Append("[<b>" + _ID + "</b>]" + "&nbsp;&nbsp;&nbsp;");
                    }
                    else
                    {
                        if (((block_page * 10) + K) != 0)
                        {
                            sbPageList.Append("<a href='?_ID=" + (block_page * 10 + K) + "'>" + (block_page * 10 + K) + "</a>");
                            sbPageList.Append("&nbsp;&nbsp;&nbsp;");
                        }
                    }
                }
            }  //for迴圈 end

            if ((block_page < (Pages / 10)) & (Pages >= (((block_page + 1) * 10) + 1)))
            {
                sbPageList.Append("&nbsp;&nbsp;<a href='?id=" + ((block_page + 1) * 10 + 1) + "'>  [>>後十頁]  </a>");
            }
            sbPageList.Append("</div>");

            ViewBag.PageList = sbPageList.ToString();
            return View(ListAll.ToList());
        }

        public IActionResult IndexPage3(int? _ID = 1)
        {
            int PageSize = 5;
            int NowPageCnt = 0;
            if(_ID > 0 || string.IsNullOrEmpty(_ID.ToString()))
                NowPageCnt = (int)(PageSize * (_ID - 1));


            var ListAll = (from u in _db.UserTables
                           orderby u.UserId
                           select u);
            if (!ListAll.Any())
                return NotFound();
            else
            {
                // 需搭配根目錄底下的PaginatedList.cs。       .AsNoTracking() -- 抓最新的資料，避免被快取（緩存 Cached）
                // 將查詢結果（ListAll，多筆資料）轉換成「單一頁面」的內容。 該單一頁面會傳遞至檢視畫面。
                return View(PaginatedList<UserTable>.PagerCreate(ListAll.AsNoTracking(), _ID ?? 1, PageSize));
                // C# 8.0起的新功能。如果 Null 聯合運算子 ?? 不是 null，會傳回其左方運算元的值；否則它會評估右方運算元，並傳回其結果。
            }
        }

        [HttpPost]
        public IActionResult Search(string _SearchWord)
        {
            //return Content("<h3> 檢視頁面傳來的 -- " + _SearchWord + "</h3>");
            ViewData["SW"] = _SearchWord;

            if(string.IsNullOrEmpty(_SearchWord) && ModelState.IsValid)
            {
                return Content("請輸入「關鍵字」才能搜尋");
            }

            #region 第一種寫法
            IQueryable<UserTable> ListAll = from ut in _db.UserTables
                                            where ut.UserName.Contains(_SearchWord)
                                            select ut;
            //IQueryable<UserTable> ListAll = from ut in _db.UserTables
            //                                where ut.UserName == _SearchWord
            //                                select ut;
            if (!ListAll.Any())
                return NotFound();
            return View(ListAll.ToList());
            #endregion

            #region 第二種寫法
            //// .AsNoTracking()需搭配 System.Data.Entity命名空間。 https://dotblogs.com.tw/wasichris/2015/03/29/150868
            //// 避免使用快取（緩存、cache）的數據，直接查詢DB內 "最新" 資料。請勿搭配 .SaveChange()使用。
            //// 優點：可以查詢到最新資料。  缺點：沒有快取，速度會慢一點點。
            //IQueryable<UserTable> ListAll = (from ut in _db.UserTables
            //                                 select ut).AsNoTracking();
            //if (!ListAll.Any())
            //    return NotFound();
            //return View(ListAll.Where(a => a.UserName.Contains(_SearchWord)));
            #endregion


        }

        public IActionResult SearchIndex()
        {
              return View(); 
        }

        
        public IActionResult Search2(string _ID)
        {
            //return Content("<h3> 檢視頁面傳來的 -- " + _ID + "</h3>");
            ViewData["SW"] = _ID;

            if (string.IsNullOrEmpty(_ID) && ModelState.IsValid)
            {
                return Content("請輸入「關鍵字」才能搜尋");
            }

            #region 第一種寫法
            IQueryable<UserTable> ListAll = from ut in _db.UserTables
                                            where ut.UserName.Contains(_ID)
                                            select ut;
            //IQueryable<UserTable> ListAll = from ut in _db.UserTables
            //                                where ut.UserName == _SearchWord
            //                                select ut;
            if (!ListAll.Any())
                return NotFound();
            return View(ListAll.ToList());
            #endregion

            #region 第二種寫法
            //// .AsNoTracking()需搭配 System.Data.Entity命名空間。 https://dotblogs.com.tw/wasichris/2015/03/29/150868
            //// 避免使用快取（緩存、cache）的數據，直接查詢DB內 "最新" 資料。請勿搭配 .SaveChange()使用。
            //// 優點：可以查詢到最新資料。  缺點：沒有快取，速度會慢一點點。
            //IQueryable<UserTable> ListAll = (from ut in _db.UserTables
            //                                 select ut).AsNoTracking();
            //if (!ListAll.Any())
            //    return NotFound();
            //return View(ListAll.Where(a => a.UserName.Contains(_SearchWord)));
            #endregion
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details(int? _ID=1)
        {
            if (!_ID.HasValue || _ID == null)
            {
                //return Content("沒有輸入文章編號（_ID）");
                return new StatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);
            }
                

            IQueryable<UserTable> ListOne = from _userTable in _db.UserTables
                                            where _userTable.UserId == _ID
                                            select _userTable;
            //if (!ListOne.Any())
            //{    // 找不到這一筆記錄
            //     ////    return NotFound();
            //    return Content(" ** Sorry! 找不到任一筆記錄 ** ");
            //}
            //else
            //{
            //    return View(ListOne.FirstOrDefault());
            //}


            IQueryable<UserTable> ListOne2 = _db.UserTables.Where(x => x.UserId == _ID);
            //if (!ListOne2.Any())
            //{    // 找不到這一筆記錄
            //     ////    return NotFound();
            //    return Content(" ** Sorry! 找不到任一筆記錄 ** ");
            //}
            //else
            //{
            //    return View(ListOne2.FirstOrDefault());
            //}

            SqlParameter _IDParameter = new SqlParameter("IDParameter", _ID);
            var ListOne3 = _db.UserTables.FromSqlRaw("select * from UserTable with(nolock) where UserID=@IDParameter", _IDParameter)
                                  .OrderBy(x => x.UserId)
                                  .FirstOrDefault();
            if (ListOne3 == null)
            {   // 找不到這一筆記錄
                return NotFound();
            }
            else
            {
                return View(ListOne3);
            }
        }

        public async Task<IActionResult> List()
        {
            #region 第一種方式
            IQueryable<UserTable> ListAll = from _userTabel in _db.UserTables
                                 select _userTabel;
            if (!ListAll.Any())
                return NotFound();
            else
                return View(await ListAll.ToListAsync());
            #endregion

            #region 第二種方式
            //if (_db.UserTables == null)
            //    return Content(" ** Sorry! 找不到任一筆記錄 ** ");
            //else
            //{
            //    return View(await _db.UserTables.ToListAsync());
            //}
            #endregion
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}