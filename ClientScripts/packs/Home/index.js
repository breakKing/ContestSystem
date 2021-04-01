import {createApp} from 'vue'
import MainComponent from "../../components/MainComponent";
import store from "../../store/index";
import router from "../../router/index";
import axios from 'axios'
import VueAxios from 'vue-axios'

const app = createApp({
    components: {
        MainComponent
    },
    template: '<main-component></main-component>'
})
app.use(VueAxios, axios)
app.use(store)
app.use(router)
app.mount('#app')