import {createRouter, createWebHistory} from 'vue-router'
import store from '../store/index'
import LoginPage from '../views/LoginPage'
import RegisterPage from '../views/RegisterPage'
import RolesPage from '../views/RolesPage'
import AdminPage from '../views/AdminPage'
import ManagerPage from '../views/ManagerPage'
import UserPage from '../views/UserPage'
import UsersListComponent from "../components/admin/UsersListComponent";

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
                    return {name: 'Login'}
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
                        route_name = 'UserHome';
                        break
                }
                return {name: route_name}
            },
            meta: {
                authorize: true
            }
        },
        {
            path: '/admin',
            name: 'AdminHome',
            component: AdminPage,
            meta: {
                authorize: ['admin']
            },
            children:[
                {
                    path: 'users',
                    name: 'AdminUsersList',
                    component: UsersListComponent
                }
            ],
        },
        {
            path: '/manager',
            name: 'ManagerHome',
            component: ManagerPage,
            meta: {
                authorize: ['manager']
            }
        },
        {
            path: '/user',
            name: 'UserHome',
            component: UserPage,
            meta: {
                authorize: ['user']
            }
        },
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
            return next({name: 'Login', query: {returnUrl: to.path}})
        }

        if (authorize.length && !authorize.includes(store.getters.currentRole)) {
            // нет нужной роли
            store.dispatch('globalAlert', {message: 'Вы куда это?)'})
            return next(false)
        }
    }
    next()
})
export default router
