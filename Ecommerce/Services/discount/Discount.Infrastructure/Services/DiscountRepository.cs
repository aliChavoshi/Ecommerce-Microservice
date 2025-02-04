using Dapper;
using Discount.Core.Entities;
using Discount.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.Infrastructure.Services;

public class DiscountRepository(IConfiguration configuration) : IDiscountRepository
{
    private readonly string? _connectionString = configuration.GetValue<string>("DatabaseSettings:ConnectionString");

    public async Task<Coupon> GetDiscount(string productName)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(
            "SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });
        return coupon ?? new Coupon
            { Amount = 0, Description = "no discount available", ProductName = "No Discount" };
    }

    public async Task<bool> CreateDiscount(Coupon coupon)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var affected = await connection.ExecuteAsync(
            "INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
            new { coupon.ProductName, coupon.Description, coupon.Amount });

        return affected > 0;
    }

    public async Task<bool> UpdateDiscount(Coupon coupon)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var affected = await connection.ExecuteAsync(
            "UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount WHERE Id=@Id",
            new { coupon.ProductName, coupon.Description, coupon.Amount, coupon.Id });

        return affected > 0;
    }

    public async Task<bool> DeleteDiscount(string productName)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var affected = await connection.ExecuteAsync(
            "DELETE FROM Coupon WHERE ProductName = @ProductName",
            new { ProductName = productName });

        return affected > 0;
    }
}