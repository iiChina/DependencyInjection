using DependencyInversion.Models;
using DependencyInversion.Repositories.Contracts;
using DependencyInversion.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DependencyInversion.Controllers;

public class OrderController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IDeliveryFeeService _deliveryService;
    private readonly IPromoCodeRepository _promoCodeRepository;

    public OrderController(
        ICustomerRepository customerRepository, 
        IDeliveryFeeService deliveryService,
        IPromoCodeRepository promoCodeRepository)
    {
        _customerRepository = customerRepository;
        _deliveryService = deliveryService;
        _promoCodeRepository = promoCodeRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Place(string customerId, string zipCode, string promoCode)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);

        if(customer == null)
            return NotFound();

        var deliveryFee = await _deliveryService.GetDeliveryFeeAsync(zipCode);
        var cupon = await _promoCodeRepository.GetPromoCodeAsync(promoCode);
        var discount = cupon?.Value ?? 0M;

        var order = new Order(deliveryFee, discount, new List<Product>());
        return Ok($"Pedido {order.Code} gerado com sucesso!");
    }
}