new Vue({
  el: '#cart',
  data: {
    products: [],
    totalPrice: 0
  },
  mounted: function () {
    axios
      .post('/PersonalArea/CartGetCartProducts')
      .then(response => {
        this.products = response.data;
      });
  },
  watch: {
    quantity: function () {
      this.price = this.quantity * this.$refs.priceOfUnit.value
    }
  }

})