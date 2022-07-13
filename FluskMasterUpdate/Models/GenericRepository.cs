using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace FluskMasterUpdate.Models
{
    public class GenericRepository<TEntity> : IDisposable where TEntity : class
    {
        private DbContext context;
        private DbSet<TEntity> dbSet;
        public int TabloTipi { get; set; }
        public string ProsedurAdi { get; set; }
        public object ParametreDegeri { get; set; }
        public string ParametreAdi { get; set; }


        public GenericRepository()
        {
            this.context = new FluskMasterUpdateEntities1();
            this.dbSet = context.Set<TEntity>();
        }
        public virtual TEntity GetLast()
        {
            var model = Get(Size: 1).FirstOrDefault();
            if (model == null)
            {
                return (TEntity)Activator.CreateInstance(typeof(TEntity));
            }
            return model;
        }
        public virtual List<TEntity> Get(out int count, Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "",
            string Sort = "Id", string Order = "desc", int Page = 1, int Size = 25)
        {
            try
            {
                if (HttpContext.Current.Request["Sort"] != null) { Sort = HttpContext.Current.Request["Sort"]; }
                if (HttpContext.Current.Request["Order"] != null) { Order = HttpContext.Current.Request["Order"]; }
                if (HttpContext.Current.Request["Page"] != null) { Page = Convert.ToInt32(HttpContext.Current.Request["Page"].ToString()); }
                if (HttpContext.Current.Request["Size"] != null) { Size = Convert.ToInt32(HttpContext.Current.Request["Size"].ToString()); }
            }
            catch (Exception)
            {
            }


            Page = Page <= 0 ? 1 : Page;
            Size = Size <= 1 ? 1 : Size;

            if (Order == "asc")
            {
                var query = ApplyOrder(filter, Sort, "OrderBy");
                count = query.Count();
                return query.Skip((Page - 1) * Size).Take(Size).AsEnumerable().ToList();
            }
            else
            {
                var query = ApplyOrder(filter, Sort, "OrderByDescending");
                count = query.Count();
                return query.Skip((Page - 1) * Size).Take(Size).AsEnumerable().ToList();
            }

        }

        public virtual List<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "",
            string Sort = "Id", string Order = "desc", int Page = 1, int Size = 25)
        {
            try
            {
                if (HttpContext.Current.Request["Sort"] != null) { Sort = HttpContext.Current.Request["Sort"]; }
                if (HttpContext.Current.Request["Order"] != null) { Order = HttpContext.Current.Request["Order"]; }
                if (HttpContext.Current.Request["Page"] != null) { Page = Convert.ToInt32(HttpContext.Current.Request["Page"].ToString()); }
                if (HttpContext.Current.Request["Size"] != null) { Size = Convert.ToInt32(HttpContext.Current.Request["Size"].ToString()); }
            }
            catch (Exception)
            {
            }

            Page = Page <= 0 ? 1 : Page;
            Size = Size <= 1 ? 1 : Size;

            if (Order == "asc")
            {
                var query = ApplyOrder(filter, Sort, "OrderBy");
                return query.Skip((Page - 1) * Size).Take(Size).AsEnumerable().ToList();
            }
            else
            {
                var query = ApplyOrder(filter, Sort, "OrderByDescending");
                return query.Skip((Page - 1) * Size).Take(Size).AsEnumerable().ToList();
            }

        }

        public virtual List<TEntity> GetLazyLoading(Expression<Func<TEntity, bool>> filter = null,
           string Sort = "Id", string Order = "desc", int Page = 1, int Size = 25)
        {
            context.Configuration.LazyLoadingEnabled = false;
            Page = Page <= 0 ? 1 : Page;
            Size = Size <= 1 ? 1 : Size;

            if (Order == "asc")
            {
                var query = ApplyOrder(filter, Sort, "OrderBy");
                return query.Skip((Page - 1) * Size).Take(Size).AsEnumerable().ToList();
            }
            else
            {
                var query = ApplyOrder(filter, Sort, "OrderByDescending");
                return query.Skip((Page - 1) * Size).Take(Size).AsEnumerable().ToList();
            }

        }

        public virtual List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "")
        {
            var query = ApplyOrder(filter, "desc", "OrderByDescending");
            return query.AsEnumerable().ToList();
        }

        public virtual List<TEntity> GetAllLazyLoading(Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "")
        {
            context.Configuration.LazyLoadingEnabled = false;
            var query = ApplyOrder(filter, "desc", "OrderByDescending");
            return query.AsEnumerable().ToList();
        }
        public virtual List<TEntity> GetSub(Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "")
        {
            var query = ApplyOrder(filter, "desc", "OrderByDescending");
            return query.AsEnumerable().ToList();
        }
        public int Count(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

            if (filter != null)
                query = query.Where(filter);
            return query.Count();
        }

        IOrderedQueryable<TEntity> ApplyOrder(Expression<Func<TEntity, bool>> predicate, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(TEntity);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            try
            {
                foreach (string prop in props)
                {
                    PropertyInfo pi = type.GetProperty(prop);
                    expr = Expression.Property(expr, pi);
                    type = pi.PropertyType;
                }
            }
            catch (Exception)
            {
                string[] prophata = { "Id" };
                foreach (string prop in prophata)
                {
                    PropertyInfo pi = type.GetProperty(prop);
                    expr = Expression.Property(expr, pi);
                    type = pi.PropertyType;
                }
            }

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(TEntity), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);
            IQueryable<TEntity> sour = GetQuery(predicate);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(TEntity), type)
                    .Invoke(null, new object[] { sour, lambda });
            return (IOrderedQueryable<TEntity>)result;

        }

        IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            if (TabloTipi == 0)
            {
                IQueryable<TEntity> query = dbSet;

                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProperty);

                if (filter != null)
                    query = query.Where(filter);
                return query;
            }
            else
            {
                if (filter != null)
                {
                    var result = context.Database.SqlQuery<TEntity>(ProsedurAdi, ParametreDegeri).AsQueryable();

                    result = result.Where(filter);
                    return result;
                }
                else
                {
                    if (ParametreDegeri != null)
                    {
                        var param3 = new SqlParameter
                        {
                            ParameterName = ParametreAdi,
                            SqlDbType = SqlDbType.Int,
                            Direction = ParameterDirection.Input,
                            Value = ParametreDegeri
                        };

                        var result = context.Database.SqlQuery<TEntity>("exec " + ProsedurAdi + " " + ParametreAdi, param3).AsQueryable();
                        return result;
                    }
                    else
                    {
                        var result = context.Database.SqlQuery<TEntity>(ProsedurAdi).AsQueryable();
                        return result;
                    }
                }
            }
        }

        public virtual TEntity GetById(object id)
        {
            var model = dbSet.Find(id);
            if (model == null)
            {
                return (TEntity)Activator.CreateInstance(typeof(TEntity));
            }
            return model;
        }

        public virtual TEntity GetByIdLazyLoading(object id)
        {
            context.Configuration.LazyLoadingEnabled = false;
            var model = dbSet.Find(id);
            if (model == null)
            {
                return (TEntity)Activator.CreateInstance(typeof(TEntity));
            }
            return model;
        }

        public virtual TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = dbSet;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            return query.FirstOrDefault(filter);
        }
        public virtual void InsertOrUpdate(TEntity entity, int Id)
        {
            if (Id > 0)
            {
                Update(entity);
            }
            else
            {
                Insert(entity);
            }
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
            Save();
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            if (entityToDelete != null)
            {
                Delete(entityToDelete);
            }           
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
            Save();
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            if (context.Entry(entityToUpdate).State == EntityState.Detached)
            {
                context.Set<TEntity>().Attach(entityToUpdate);
                context.Entry(entityToUpdate).State = EntityState.Modified;
            }
            else
            {
                context.Entry(entityToUpdate).State = EntityState.Modified;
            }
            Save();
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public static class Utility
    {
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
    }
    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }
    public class ReplaceVisitor : ExpressionVisitor
    {
        private readonly Expression from, to;
        public ReplaceVisitor(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }
        public override Expression Visit(Expression node)
        {
            return node == from ? to : base.Visit(node);
        }
    }
    public class DynamicExpression
    {
        private static MethodInfo miTL = typeof(String).GetMethod("ToLower", Type.EmptyTypes);
        private static MethodInfo miC = typeof(String).GetMethod("Contains", new Type[] { typeof(String) });
        private static MethodInfo miS = typeof(Int32).GetMethod("ToString", Type.EmptyTypes);
        public static Expression<Func<TEntity, bool>> Sorgu<TEntity>(List<string> alanadi, List<object> alanindegeri, List<string> kosullar, int KosulBagi) where TEntity : class
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "x");
            Expression equal = null;
            Expression dynamiclambda = null;
            for (int i = 0; i < alanadi.Count; i++)
            {
                if (alanindegeri[i] != "")
                {
                    Type type = typeof(TEntity);
                    Expression expr = parameter;
                    try
                    {
                        string[] props = alanadi[i].Split('.');
                        foreach (string prop in props)
                        {
                            PropertyInfo pi = type.GetProperty(prop);
                            expr = Expression.Property(expr, pi);
                            type = pi.PropertyType;
                        }
                    }
                    catch (Exception)
                    {
                    }

                    //kosullar
                    //value="1">İçerir :Contains
                    //value="2">Eşittir :Equal
                    //value="3">Eşit Değil :notEqual
                    //value="4">Büyüktür
                    //value="5">Küçüktür
                    //value="6">Büyük Eşittir
                    //value="7">Küçük Eşittir

                    if (expr.Type.Name == "Int32")
                    {
                        #region Int32 koşulları
                        //if (kosullar[i] == "1")
                        //{
                        //    //Expression constExp = Expression.Constant(alanindegeri[i].ToString().ToLower());
                        //    //var like = typeof(SqlMethods).GetMethod("Like", new Type[] { typeof(string), typeof(string) });
                        //    //equal = Expression.Call(expr, miS);
                        //    ////equal = Expression.Call(equal, miC, constExp);
                        //    //MethodCallExpression methodExp =
                        //    //      Expression.Call(null, like, equal, constExp);
                        //    //equal = methodExp;


                        //    var property4 = type.GetProperty("Plaka");

                        //    var parameter4 = Expression.Parameter(type, "x");
                        //    var propertyAccess = Expression.MakeMemberAccess(parameter4, );
                        //    var constant = Expression.Constant("%6%");

                        //    var like = typeof(SqlMethods).GetMethod("Like",
                        //               new Type[] { typeof(string), typeof(string) });
                        //    MethodCallExpression methodExp =
                        //          Expression.Call(null, like, propertyAccess, constant);

                        //    equal = methodExp;
                        //}
                        if (kosullar[i] == "2")
                        {
                            int val;
                            int.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.Equal(expr, soap);
                        }
                        else if (kosullar[i] == "3")
                        {
                            int val;
                            int.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.NotEqual(expr, soap);
                        }
                        else if (kosullar[i] == "4")
                        {
                            int val;
                            int.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.GreaterThan(expr, soap);
                        }
                        else if (kosullar[i] == "5")
                        {
                            int val;
                            int.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.LessThan(expr, soap);
                        }
                        else if (kosullar[i] == "6")
                        {
                            int val;
                            int.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.GreaterThanOrEqual(expr, soap);
                        }
                        else
                        {
                            int val;
                            int.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.LessThanOrEqual(expr, soap);
                        }
                        #endregion
                    }
                    else if (expr.Type.Name == "Int64")
                    {
                        #region Int64 koşulları
                        if (kosullar[i] == "2")
                        {
                            long val;
                            long.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.Equal(expr, soap);
                        }
                        else if (kosullar[i] == "3")
                        {
                            long val;
                            long.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.NotEqual(expr, soap);
                        }
                        else if (kosullar[i] == "4")
                        {
                            long val;
                            long.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.GreaterThan(expr, soap);
                        }
                        else if (kosullar[i] == "5")
                        {
                            long val;
                            long.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.LessThan(expr, soap);
                        }
                        else if (kosullar[i] == "6")
                        {
                            long val;
                            long.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.GreaterThanOrEqual(expr, soap);
                        }
                        else
                        {
                            long val;
                            long.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.LessThanOrEqual(expr, soap);
                        }
                        #endregion
                    }
                    else if (expr.Type.Name == "DateTime")
                    {
                        #region DateTime koşulları
                        if (kosullar[i] == "2")
                        {
                            DateTime val;
                            DateTime.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.Equal(expr, soap);
                        }
                        else if (kosullar[i] == "3")
                        {
                            DateTime val;
                            DateTime.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.NotEqual(expr, soap);
                        }
                        else if (kosullar[i] == "4")
                        {
                            DateTime val;
                            DateTime.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.GreaterThan(expr, soap);
                        }
                        else if (kosullar[i] == "5")
                        {
                            DateTime val;
                            DateTime.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.LessThan(expr, soap);
                        }
                        else if (kosullar[i] == "6")
                        {
                            DateTime val;
                            DateTime.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.GreaterThanOrEqual(expr, soap);
                        }
                        else
                        {
                            DateTime val;
                            DateTime.TryParse(alanindegeri[i].ToString(), out val);
                            var soap = Expression.Constant(val);
                            equal = Expression.LessThanOrEqual(expr, soap);
                        }
                        #endregion
                    }
                    else if (expr.Type.Name == "String")
                    {
                        #region String koşulları
                        Expression constExp = Expression.Constant(alanindegeri[i].ToString().ToLower());
                        if (kosullar[i] == "1")
                        {
                            equal = Expression.Call(expr, miTL);
                            equal = Expression.Call(equal, miC, constExp);
                        }
                        else if (kosullar[i] == "2")
                        {
                            equal = Expression.Call(expr, miTL);
                            equal = Expression.Equal(equal, constExp);
                        }
                        else if (kosullar[i] == "3")
                        {
                            equal = Expression.Call(expr, miTL);
                            equal = Expression.NotEqual(equal, constExp);
                        }
                        #endregion
                    }

                    if (null == dynamiclambda)
                    {
                        dynamiclambda = equal;
                    }
                    else
                    {
                        if (KosulBagi == 1)
                        {
                            dynamiclambda = Expression.AndAlso(dynamiclambda, equal);
                        }
                        else
                        {
                            dynamiclambda = Expression.OrElse(dynamiclambda, equal);
                        }
                    }
                }
            }
            if (null != dynamiclambda)
            {
                Expression<Func<TEntity, bool>> predicate =
                   Expression.Lambda<Func<TEntity, bool>>(dynamiclambda, parameter);
                return predicate;
            }
            return null;
        }

    }
}