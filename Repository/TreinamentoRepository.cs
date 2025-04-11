using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Projeto_Trainee_Hub.Models;
namespace Projeto_Trainee_Hub.Repository
{
    
    public class TreinamentoRepository
    {
        private readonly MasterContext _context;
        public TreinamentoRepository(MasterContext context)
        {
            _context = context;
        }

        public async Task<List<Treinamento>> GetAllAsync()
        {
            return await _context.Treinamentos.ToListAsync();
        }

        public async Task<Treinamento?> ObterPorIdAsync(int id)
        {
            return await _context.Treinamentos
                .Include(t => t.IdCriadorNavigation)
                .Include(t => t.IdEmpresaNavigation)
                .FirstOrDefaultAsync(t => t.IdTreinamentos == id);
        }
        public async Task<Treinamento?> ObterPorIdSemRastreamentoAsync(int id)
        {
            return await _context.Treinamentos.AsNoTracking()
                .FirstOrDefaultAsync(t => t.IdTreinamentos == id);
        }

        public async Task <IEnumerable<Treinamento?>> ObterPorIdCriadorAsync(int id)
        {
            return await _context.Treinamentos
                .Include(t => t.IdCriadorNavigation)
                .Include(t => t.IdEmpresaNavigation)
                .Include(t => t.IdCriadorNavigation.IdSetorNavigation)
                .Where(t => t.IdCriador == id)
                .ToListAsync();
        }


        public async Task<IEnumerable<Treinamento>> FindAsync(Expression<Func<Treinamento, bool>> predicate)
        {
            return await _context.Treinamentos.Where(predicate).ToListAsync();
        }

        public async Task<Treinamento> CriarTreinamento(Treinamento treinamento)
        {
            _context.Treinamentos.Add(treinamento);
            await _context.SaveChangesAsync();
            return treinamento;
        }

        public async Task UpdateAsync(Treinamento entity)
        {
            _context.Treinamentos.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Treinamentos.FindAsync(id);
            if (entity != null)
            {
                _context.Treinamentos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public void Adicionar(Treinamento treinamento)
        {
            _context.Treinamentos.Add(treinamento);
        }

        public void Salvar()
        {
            _context.SaveChanges();
        }
    }
}
