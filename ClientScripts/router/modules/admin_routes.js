import AdminPage from "../../views/admin/AdminPage";
import UsersListComponent from "../../components/admin/UsersListComponent";

export default {
    path: '/admin',
    name: 'AdminHome',
    component: AdminPage,
    meta: {
        authorize: ['admin']
    },
    children: [
        {
            path: 'users',
            name: 'AdminUsersList',
            component: UsersListComponent,
            meta: {
                authorize: ['admin']
            },
        }
    ],
}