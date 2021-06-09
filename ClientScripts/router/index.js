import {createRouter, createWebHistory} from 'vue-router'
import store from '../store/index'
import LoginPage from '../views/LoginPage'
import RegisterPage from '../views/RegisterPage'
import RolesPage from '../views/RolesPage'
import UserStarterPage from '../views/user/UserStarterPage'
import UserRoutes from "./modules/user_routes";
import ManagerRoutes from "./modules/moder_routes";
import AdminRoutes from "./modules/admin_routes";
import NProgress from 'nprogress'
import 'nprogress/nprogress.css';

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/',
            name: 'Home',
            redirect: (to) => {
                // redirect синхронный
                (async () => {
                    await store.dispatch('initAuth') // ensure auth synced with server
                })()
                if (!store.getters.isAuthenticated) {
                    return {name: 'UserStarterPage'}
                }
                let current_role = store.getters.currentRole
                if (!current_role) {
                    return {name: 'RoleSelector'}
                }
                let route_name;
                // разбираем на страницы по ролям
                switch (current_role) {
                    case 'admin':
                        route_name = 'AdminHome';
                        break
                    case 'manager':
                        route_name = 'ManagerHome';
                        break
                    default: // user
                        route_name = 'UserStarterPage';
                        break
                }
                return {name: route_name}
            },
            meta: {
                authorize: true
            }
        },
        AdminRoutes,
        ManagerRoutes,
        UserRoutes,
        {
            path: '/roles',
            name: 'RoleSelector',
            component: RolesPage,
            meta: {
                authorize: true
            }
        },
        {
            path: '/login',
            name: 'Login',
            component: LoginPage,
            props: true,
        },
        {
            path: '/register',
            name: 'Register',
            component: RegisterPage,
            props: true,
        },
    ],
})

router.beforeResolve((to, from, next) => {
    if (to.name) {
        // Запустить отображение загрузки
        NProgress.start()
    }
    next()
})

router.afterEach(() => {
    // Завершить отображение загрузки
    NProgress.done()
})

router.beforeEach(async (to, from, next) => {
    await store.dispatch('initAuth') // ensure auth synced with server
    let isAuthenticated = store.getters.isAuthenticated
    if (isAuthenticated) {
        if (['Login', 'Register'].includes(to.name)) {
            return next({name: 'Home'})
        }
    }
    const {authorize} = to.meta;
    if (authorize) {
        if (!isAuthenticated) {
            return next({name: 'UserStarterPage', query: {returnUrl: to.path}})
        }

        if (authorize.length && !authorize.includes(store.getters.currentRole)) {
            // нет нужной роли
            await store.dispatch('globalAlert', {message: 'Вы куда это?)'})
            return next(false)
        }
    }
    next()
})
export default router
