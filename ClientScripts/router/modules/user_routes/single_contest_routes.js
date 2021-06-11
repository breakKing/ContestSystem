import ContestPage from "../../../views/user/contests/single_contest/ContestPage";
import ContestInformationComponent from "../../../components/user/contests/ContestInformationComponent";

export default {
    path: 'contest',
    name: 'MainContestPage',
    component: ContestPage,
    meta: {
        authorize: ['user']
    },
    children: [
        {
            path: ':contest_id',
            name: 'ContestPage',
            component: ContestInformationComponent,
            meta: {
                authorize: ['user']
            },
        },
    ]
}
