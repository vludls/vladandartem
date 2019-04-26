new Vue({
el: '#cart',
data: {
	products: [],
	totalPrice: 0,
	modalId: 'id',
	productId: '',
	selectProduct: '',
	index: '',
},
mounted: function () {
	axios
	.post('/PersonalArea/CartGetCartProducts')
	.then(response => {
		this.products = response.data;
		for(var i = 0; i < this.products.length; i++ ) {
		this.totalPrice += this.products[i].Product.Price * this.products[i].Count;
		};
	});
},
methods: {
	SetTotalPrice: function () {
	this.totalPrice = 0;
	for(var i = 0; i < this.products.length; i++ ) {
		this.totalPrice += this.products[i].Product.Price * this.products[i].Count;
	}
	},
	SaveQuantity: function (productId, productCount, productCountOnStore) {
		axios
		.post("/Home/CartChangeProductNum", null, { 
			params: {
				id: productId,
				count: productCount
			}
		})
		.then(response => {
			productCountOnStore.Count = response.data;
		})
	},
	activateModal: function (productId, productName, index) {
		this.modalId = 'id' + productId;
		this.selectProduct = productName;
		this.productId = productId;
		this.index = index
	},
	deleteProduct: function (event) {
		axios
		.post('/Home/RemoveProductCart', null, { 
			params: { 
				cartProductId: this.productId
			} 
		})
		.then(response => {
			this.products.splice(this.index, 1);
			this.SetTotalPrice();
		});
	event.preventDefault();
	},
	closeModal: function() {
		$('.close-modal').trigger('click');
	}
}
})