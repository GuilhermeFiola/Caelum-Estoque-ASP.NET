using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaelumEstoque.DAO;
using CaelumEstoque.Models;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CaelumEstoque.Controllers
{
    public class ProdutosController : Controller
    {

        String stringDeConexao = ConfigurationManager.ConnectionStrings["EstoqueContext"].ConnectionString.ToString();

        // GET: Produtos
        [Route("produtos", Name="ListaProdutos")]
        public ActionResult Index()
        {
            //ProdutosDAO dao = new ProdutosDAO();
            //IList<Produto> produtos = dao.Lista();
            //ViewBag.Produtos = produtos;

            IList<Produto> produtos = new List<Produto>();

            DataTable table = Select("select * from produtoes", "produtos", stringDeConexao);

            foreach (DataRow row in table.Rows)
            {
                Produto produto = new Produto();
                produto.Id = Int32.Parse(row["Id"].ToString());
                produto.Nome = row["Nome"].ToString();
                produto.Descricao = row["Descricao"].ToString();
                produto.Quantidade = Int32.Parse(row["Quantidade"].ToString());
                produto.Preco = float.Parse(row["Preco"].ToString());
                produto.CategoriaId = Int32.Parse(row["CategoriaId"].ToString());
                produtos.Add(produto);
            }

            return View(produtos);
        }

        [Route("produtos/form", Name = "FormProdutos")]
        public ActionResult Form()
        {
            CategoriasDAO categoriaDAO = new CategoriasDAO();
            IList<CategoriaDoProduto> categorias = categoriaDAO.Lista();
            Produto produto = new Produto();
            ViewBag.Categorias = categorias;
            ViewBag.Produtos = produto;
            return View();
        }

        [HttpPost]
        public ActionResult Adiciona(Produto produto)
        {
            int idInformatica = 1;
            if (produto.Preco <= 100 && produto.CategoriaId.Equals(idInformatica))
            {
                ModelState.AddModelError("produto.InformaticaComPrecoInvalido", "Produtos da categoria informática não podem ter preço menor que R$ 100,00.");
            }

            /*if (ModelState.IsValid)
            {*/
                //ProdutosDAO produtosDAO = new ProdutosDAO();
                //produtosDAO.Adiciona(produto);
                String sql = "insert into Produtoes (Nome, Preco, CategoriaId, Descricao, Quantidade) Values ('" +
                              produto.Nome + "'," + produto.Preco + "," + produto.CategoriaId + ",'" + produto.Descricao + "'," + produto.Quantidade + ")";

                ExecutaQuery(sql, stringDeConexao);
                return Json(new {data = produto.Nome}, JsonRequestBehavior.AllowGet); //RedirectToAction("Index", "Produtos");
            //}
            /*else
            {
                CategoriasDAO categoriasDAO = new CategoriasDAO();
                IList<CategoriaDoProduto> categorias = categoriasDAO.Lista();
                ViewBag.Categorias = categorias;
                ViewBag.Produtos = produto;
                return View("Form");
            }*/
        }

        [Route("produtosvisualiza/{id}", Name="VisualizaProdutos")]
        public ActionResult Visualiza(int id)
        {
            ProdutosDAO produtosDAO = new ProdutosDAO();
            Produto produto = produtosDAO.BuscaPorId(id);
            ViewBag.Produtos = produto;
            return View();
        }

        public ActionResult DecrementaQtd(int id)
        {
            ProdutosDAO dao = new ProdutosDAO();
            Produto produto = dao.BuscaPorId(id);
            produto.Quantidade--;
            dao.Atualiza(produto);
            //return RedirectToAction("Index");
            return Json(produto);
        }

        private void ExecutaQuery(String comandoSql, String stringDeConexao)
        {
            using(SqlConnection conexao = new SqlConnection(stringDeConexao)){
                SqlCommand comando = new SqlCommand(comandoSql, conexao);
                comando.Connection.Open();
                comando.ExecuteNonQuery();
            };
        }

        private DataTable Select(String comandoSql, String nomeTabela, String stringDeConexao)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet set = new DataSet();
            DataTable table = new DataTable();
            
            using (SqlConnection conexao = new SqlConnection(stringDeConexao))
            {
                adapter.SelectCommand = new SqlCommand(comandoSql, conexao);
                adapter.Fill(set, nomeTabela);
                table = set.Tables[nomeTabela];
                adapter.Dispose();
                set.Dispose();
            }
            return table;
        }


    }
}