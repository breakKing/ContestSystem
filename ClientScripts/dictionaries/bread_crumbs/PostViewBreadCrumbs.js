export default function (post_id) {
    return [
        {
            to: {name: 'UserStarterPage',},
            breadcrumbName: 'Домой',
        },
        {
            to: {name: 'PostsPage',},
            breadcrumbName: 'Посты',
        },
        {
            to: {
                name: 'ViewPost',
                params: {
                    post_id
                },
            },
            breadcrumbName: 'Пост',
        },
    ]
}