new Vue({
  el: '#cart',
  data: {
    products: [],
    totalPrice: 0,
    modalId: 'id',
    id: '',
    selectProduct: '',
    ProductId: []
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
      });
    },
    activateModal: function (productId, productName) {
      this.modalId = 'id';
      this.modalId += productId;
      this.selectProduct = '';
      this.selectProduct += productName;
      this.id = '';
      this.id += productId
    },
    deleteUser: function (event) {
      const data = new FormData(document.querySelector('.product-delete'));
      data.append('id', this.id);
      axios
      .post('/Home/RemoveProductCart', data 
      )
      .then(response => {
          this.ProductId.push(response.data.CartProductId);
      });
      event.preventDefault();
      },
    closeModal: function() {
        $('.close-modal').trigger('click');
    }
  }
})